using AutoMapper;
using FreeSql;
using FreeSql.DatabaseModel;
using EasyOC.Core.Application;
using EasyOC.OrchardCore.RDBMS.Models;
using EasyOC.OrchardCore.RDBMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Records;
using OrchardCore.ContentManagement.Records;

using OrchardCore.Deployment.Services;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Mvc.Utilities;
using Panda.DynamicWebApi.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.RDBMS.Services
{
    public class RDBMSAppService : AppServcieBase, IRDBMSAppService
    {
        private readonly IMapper mapper;
        private readonly IContentManager _contentManager;
        private readonly IMemoryCache _memoryCache;
        private readonly IDeploymentManager _deploymentManager;

        private readonly IAuthorizationService _authorizationService;
        private readonly IContentDefinitionManager contentDefinitionManager;
        private readonly IContentFieldsValuePathProvider _contentFieldsValuePathProvider;
        public RDBMSAppService(
            IMapper mapper,
            IContentManager contentManager, IContentFieldsValuePathProvider contentFieldsValuePathProvider,
            IContentDefinitionManager contentDefinitionManager, IMemoryCache memoryCache, IAuthorizationService authorizationService, IDeploymentManager deploymentManager)
        {
            this.mapper = mapper;
            _contentManager = contentManager;
            _contentFieldsValuePathProvider = contentFieldsValuePathProvider;

            this.contentDefinitionManager = contentDefinitionManager;
            _memoryCache = memoryCache;
            _authorizationService = authorizationService;
            _deploymentManager = deploymentManager;
        }

        private async Task<ContentItem> GetConnectionConfigAsync(string connectionConfigId)
        {
            var connectionSettings = await YesSession.Query<ContentItem, ContentItemIndex>()
                                            .Where(x => x.ContentType == "DbConnectionConfig"
                                                    && (x.Published || x.Latest)
                                                    && x.ContentItemId == connectionConfigId
                                                    ).FirstOrDefaultAsync();

            return connectionSettings;
        }


        [NonDynamicMethod]
        public async Task<IFreeSql> GetFreeSqlAsync(string connectionConfigId)
        {
            var connectionObject = await GetConnectionConfigAsync(connectionConfigId);
            return FreeSqlProviderFactory.GetFreeSql(connectionObject.Content.DbConnectionConfig.DatabaseProvider.Text.Value,
                connectionObject.Content.DbConnectionConfig.ConnectionString.Text.Value);
        }
        /// <summary>
        /// Get all Connection Config
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ConnectionConfigModel>> GetAllDbConnecton()
        {
            var connectionSettings = await YesSession.Query<ContentItem, ContentItemIndex>()
                                           .Where(x => x.ContentType == "DbConnectionConfig" && (x.Published || x.Latest)).ListAsync();
            var connectionList = connectionSettings.Select(x => new ConnectionConfigModel() { ConfigName = x.DisplayText, ConfigId = x.ContentItemId });
            return connectionList;
        }

        /// <summary>
        /// 根据连接设置获取指定数据库的表信息
        /// </summary>
        /// <param name="queryTablesDto"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DbTableInfoDto>> GetAllTablesAsync([FromQuery] QueryTablesDto queryTablesDto)
        {
            try
            {
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
                    .Select(x =>
                    {
                        var mResult = new DbTableInfoDto() { ColumnsCount = x.Columns.Count };
                        x.Columns.Clear();
                        mResult = mapper.Map(x, mResult);
                        return mResult;
                    }
                    );
                return tables;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private async Task<List<DbTableInfo>> GetTablesFromCache(string ConnectionConfigId, bool DisableCache)
        {
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
            var mResult = mapper.Map<DbTableInfoDto>(result);
            mResult.ColumnsCount = result.Columns.Count;
            return mResult;
        }


        [HttpGet]
        public async Task<string> GenerateRecipeAsync(string connectionConfigId, string tableName)
        {


            IFreeSql freeSql = await GetFreeSqlAsync(connectionConfigId);
            using (freeSql)
            {

                var recrods = new List<ContentPartFieldDefinitionRecord>();
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
                    contentType.ContentTypePartDefinitionRecords = new ContentTypePartDefinitionRecord[]{ new ContentTypePartDefinitionRecord
                    {
                        Name = partName,
                        PartName =partName,
                        Settings=JObject.FromObject(
                                new {
                                    ContentTypePartSettings=new
                                    {
                                        Position="0"
                                    }
                                })
                    }};

                    var recipe = new RecipeModel()
                    {
                        name = fullName.ToSafeName(),
                        displayName = fullName,
                        description = $"update from db {fullName}",
                        issetuprecipe = false,
                        tags = Array.Empty<object>(),
                        categories = Array.Empty<object>()
                    };

                    recipe.steps = new List<Step>() { step };

                    var index = 0;
                    foreach (var item in tb.Columns)
                    {
                        var recrod = new ContentPartFieldDefinitionRecord();
                        recrod.Name = item.Name.ToSafeName();
                        recrod.Settings = JObject.FromObject(new
                        {
                            ContentPartFieldSettings = new { DisplayName = item.Name, Position = (index++).ToString() }
                        });
                        var targetFieldType = _contentFieldsValuePathProvider.GetField(item.CsType);
                        recrod.FieldName = targetFieldType.FieldName;
                        recrods.Add(recrod);
                    }

                    step.ContentParts.Add(new Contentpart
                    {
                        Name = partName,
                        Settings = new ContentpartSettings()
                        {
                            ContentPartSettings = new Contentpartsettings { Attachable = true, DefaultPosition = "0", Description = tb.Type.ToString() + " of " + fullName }
                        },
                        DispalyName = fullName,
                        ContentPartFieldDefinitionRecords = recrods.ToArray()
                    });

                    return Newtonsoft.Json.JsonConvert.SerializeObject(recipe);
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
        /// <summary>
        /// 使用JSON更新类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception> 
        public async Task ImportDeploymentPackageAsync(ImportJsonInupt model)
        {
            if (!model.Json.IsJson())
            {
                throw new ArgumentException(S["The recipe is written in an incorrect json format."]);
            }
            var tempArchiveFolder = PathExtensions.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(tempArchiveFolder);
                File.WriteAllText(Path.Combine(tempArchiveFolder, "Recipe.json"), model.Json);
                await _deploymentManager.ImportDeploymentPackageAsync(new PhysicalFileProvider(tempArchiveFolder));
            }
            finally
            {
                if (Directory.Exists(tempArchiveFolder))
                {
                    Directory.Delete(tempArchiveFolder, true);
                }
            }
        }
        public JObject GetTableInfo(RDBMSMappingConfigViewModel config)
        {
            return null;
        }

    }
}



