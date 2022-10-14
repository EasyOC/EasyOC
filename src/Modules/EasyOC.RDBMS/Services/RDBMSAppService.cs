using AutoMapper;
using EasyOC.Core.Application;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.RDBMS.Models;
using EasyOC.RDBMS.Services.Dto;
using EasyOC.RDBMS.ViewModels;
using FreeSql.DatabaseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Records;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Deployment.Services;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Mvc.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.RDBMS.Services
{

    [EOCAuthorization(OCPermissions.EditContentTypes)]
    public class RDBMSAppService : AppServiceBase, IRDBMSAppService
    {
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache; 
        private readonly IServiceProvider _serviceProvider; 
        private readonly IContentFieldsValuePathProvider _contentFieldsValuePathProvider;
        public RDBMSAppService(
            IMapper mapper, IMemoryCache memoryCache, IServiceProvider serviceProvider)
        {
             _mapper = mapper;
            _contentFieldsValuePathProvider = new ContentFieldsValuePathProvider();
            _memoryCache = memoryCache; 
            _serviceProvider = serviceProvider;
        }

        [IgnoreWebApiMethod]
        public async Task<ContentItem> GetConnectionConfigAsync(string connectionConfigId)
        {
            var connectionSettings = await YesSession.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.ContentType == "DbConnectionConfig"
                            && (x.Published || x.Latest)
                            && x.ContentItemId == connectionConfigId
                ).FirstOrDefaultAsync();

            return connectionSettings;
        }


        [IgnoreWebApiMethod]
        public async Task<IFreeSql> GetFreeSqlAsync(string connectionConfigId)
        {
            if (connectionConfigId == Constants.ShellDbName)
                return _serviceProvider.GetFreeSql();

            var connectionObject = await GetConnectionConfigAsync(connectionConfigId);
            if (connectionObject == null) { return null; }
            var providerName = (string)connectionObject.Content.DbConnectionConfig.DatabaseProvider.Text.Value;
            var connectionStr = (string)connectionObject.Content.DbConnectionConfig.ConnectionString.Text.Value;
            //无法判断 dynamic 类型 ，所以要先显示指定类型
            return _serviceProvider.GetFreeSql(providerName, connectionStr);

        }
        /// <summary>
        /// Get all Connection Config
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ConnectionConfigModel>> GetAllDbConnection()
        {
            var connectionSettings = await YesSession.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.ContentType == "DbConnectionConfig" && (x.Published)).ListAsync();
            var connectionList = connectionSettings.Select(x => new ConnectionConfigModel()
            {
                ConfigName = x.DisplayText,
                DbProvider = x.Content.DbConnectionConfig.DatabaseProvider.Text.Value,
                ConfigId = x.ContentItemId
            }).ToList();
            connectionList.Insert(0, new ConnectionConfigModel()
            {
                ConfigName = Constants.ShellDbName,
                DbProvider = Fsql.Ado.DataType.ToString(),
                ConfigId = Constants.ShellDbName
            });
            return connectionList;
        }

        /// <summary>
        /// 根据连接设置获取指定数据库的表信息
        /// </summary>
        /// <param name="queryTablesDto"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DbTableInfoDto>> GetAllTablesAsync(QueryTablesDto queryTablesDto)
        {
            if (HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                Console.WriteLine();
            }
            List<DbTableInfo> result = await GetTablesFromCache(queryTablesDto.ConnectionConfigId, queryTablesDto.DisableCache);

            if (!string.IsNullOrEmpty(queryTablesDto.FilterText))
            {
                queryTablesDto.FilterText = queryTablesDto.FilterText.ToLower().Replace("[", string.Empty).Replace("]", string.Empty);
            }

            var tables = result
                .WhereIf(!string.IsNullOrEmpty(queryTablesDto.FilterText),
                x => $"{x.Schema}.{x.Name}".ToLower().Contains(queryTablesDto.FilterText))
                .OrderBy(x => x.Schema).ThenBy(x => x.Name)
                .Take(queryTablesDto.MaxResultCount)
                .Select(x => {
                    var mResult = new DbTableInfoDto()
                    {
                        ColumnsCount = x.Columns.Count
                    };
                    x.Columns.Clear();
                    mResult = _mapper.Map(x, mResult);
                    return mResult;
                }
                );

            return tables;
        }

        private async Task<List<DbTableInfo>> GetTablesFromCache(string ConnectionConfigId, bool DisableCache)
        {
            if (ConnectionConfigId is null)
            {
                return null;
            }
            var cacheKey = $"TablesLIST_{ConnectionConfigId}";
            if (DisableCache ||
                !_memoryCache.TryGetValue<List<DbTableInfo>>
                    (cacheKey, out var result))
            {
                var freeSql = await GetFreeSqlAsync(ConnectionConfigId);
                result = freeSql.DbFirst.GetTablesByDatabase();
                _memoryCache.Set(cacheKey, result, absoluteExpiration: DateTime.Now.AddHours(4));
            }
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionConfigId"></param>
        /// <param name="tableName"> 表名，如：dbo.table1 </param>
        /// <returns></returns>
        public async Task<DbTableInfoDto> GetTableDetailsAsync(string connectionConfigId, string tableName)
        {
            var freeSql = await GetFreeSqlAsync(connectionConfigId);

            var result = freeSql.DbFirst.GetTableByName(tableName);
            var mResult = _mapper.Map<DbTableInfoDto>(result);
            mResult.ColumnsCount = result.Columns.Count;
            return mResult;
        }


        [HttpGet]
        public async Task<GenerateRecipeDto> GenerateRecipeAsync(string connectionConfigId, string tableName)
        {
            IFreeSql freeSql = await GetFreeSqlAsync(connectionConfigId);
            using (freeSql)
            {
                var records = new List<ContentPartFieldDefinitionRecord>();
                try
                {
                    var tb = freeSql.DbFirst.GetTableByName(tableName);
                    var fullName = $"{tb.Type.ToString().ToLower().ToPascalCase()}_{tb.Schema}.{tb.Name}";
                    var typeName = fullName.Replace("dbo.", string.Empty).ToPascalCase().ToSafeName();
                    var step = new Step();
                    step.name = "ContentDefinition";
                    step.ContentTypes = new List<ContentType>();
                    var contentType = new ContentType()
                    {
                        Name = typeName,
                        DisplayName = fullName,
                        Settings = JObject.Parse(@"
                                            {'ContentTypeSettings': {
                                                'Creatable': true,
                                                'Listable': true,
                                                'Draftable': true,
                                                'Versionable': true,
                                                'Securable': true
                                            }}")


                    };
                    step.ContentTypes.Add(contentType);
                    var partName = typeName;
                    contentType.ContentTypePartDefinitionRecords = new ContentTypePartDefinitionRecord[]
                    {
                        new ContentTypePartDefinitionRecord
                        {
                            Name = partName,
                            PartName = partName,
                            Settings = JObject.FromObject(
                            new
                            {
                                ContentTypePartSettings = new
                                {
                                    Position = "0"
                                }
                            })
                        }
                    };

                    var recipe = new RecipeModel()
                    {
                        name = fullName.ToSafeName(),
                        displayName = fullName,
                        description = $"update from db {fullName}",
                        issetuprecipe = false,
                        tags = Array.Empty<object>(),
                        categories = Array.Empty<object>()
                    };

                    recipe.steps = new List<Step>()
                    {
                        step
                    };

                    var index = 0;
                    foreach (var item in tb.Columns)
                    {
                        var record = new ContentPartFieldDefinitionRecord();
                        record.Name = item.Name.ToSafeName();
                        record.Settings = JObject.FromObject(new
                        {
                            ContentPartFieldSettings = new
                            {
                                DisplayName = item.Name,
                                Position = (index++).ToString()
                            }
                        });
                        var targetFieldType = _contentFieldsValuePathProvider.GetField(item.CsType);
                        if (targetFieldType == null)
                        {
                            targetFieldType = _contentFieldsValuePathProvider.GetField(typeof(int));
                        }
                        record.FieldName = targetFieldType.FieldName;
                        records.Add(record);
                    }

                    step.ContentParts.Add(new Contentpart
                    {
                        Name = partName,
                        Settings = new ContentpartSettings()
                        {
                            ContentPartSettings = new Contentpartsettings
                            {
                                Attachable = true,
                                DefaultPosition = "0",
                                Description = tb.Type + " of " + fullName
                            }
                        },
                        DispalyName = fullName,
                        ContentPartFieldDefinitionRecords = records.ToArray()
                    });

                    return new GenerateRecipeDto(
                        connectionConfigId,
                        tableName,
                        Newtonsoft.Json.JsonConvert.SerializeObject(recipe),
                        typeName
                        )
                        ;
                }
                catch (Exception ex)
                {
                    await Notifier.ErrorAsync(H[ex.Message]);
                    //Logger.LogError(ex, ex.Message);
                    return null;
                }
            }

        }

        public IEnumerable<OrchardCoreBaseField> GetAllOrchardCoreBaseFields()
        {
            return _contentFieldsValuePathProvider.GetAllOrchardCoreBaseFields();
        }

        public JObject GetTableInfo(RDBMSMappingConfigViewModel config)
        {
            throw new NotImplementedException();
        }

    }
}