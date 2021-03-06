using EasyOC.Core.Application;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.OrchardCore.CSharpScript.Services;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Shells.Database.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Service
{
    public class DynamicIndexAppService : AppServiceBase, IDynamicIndexAppService
    {
        public const string DefaultTableNameTemplate = "{0}DIndex_{1}";
        public const string DefaultEntityNameTemplate = "{0}DIndex";
        public const string DefaultNamespace = "EasyOC.DynamicTypeIndex.IndexModels";
        private readonly Dictionary<string, Type> _typesCache = new Dictionary<string, Type>();

        private readonly ICSharpScriptProvider _cSharpScriptProvider;
        private readonly ConcurrentDictionary<string, Task<DynamicIndexConfigModel>> _cachedTypeConfigurations;
        private readonly IStore _store;
        private readonly IShellConfiguration _shellConfiguration;

        public string GetDynamicIndexTableName(string typeName)
        {
            var shellConfig = ShellScope.Current.ServiceProvider.GetRequiredService<IShellConfiguration>();
            var dbOptions = shellConfig.Get<DatabaseShellsStorageOptions>();
            return string.Format(DefaultTableNameTemplate, dbOptions.TablePrefix, typeName);
        }

        public DynamicIndexAppService(IStore store, IShellConfiguration shellConfiguration, IMemoryCache memoryCache,
            ICSharpScriptProvider cSharpScriptProvider)
        {
            _store = store;
            _shellConfiguration = shellConfiguration;
            _cSharpScriptProvider = cSharpScriptProvider;
            //_cachedTypeConfigurations = _memoryCache.GetOrCreate("CachedTypeConfigurations", entry => new ConcurrentDictionary<string, DynamicIndexCachePakage>());
            _cachedTypeConfigurations = memoryCache.GetOrCreate("CachedTypeConfigurations",
            entry => new ConcurrentDictionary<string, Task<DynamicIndexConfigModel>>());
        }

        public async Task<DynamicIndexConfigModel> GetDynamicIndexConfigOrDefaultAsync(string typeName)
        {
            var config = await GetDynamicIndexConfigAsync(typeName);
            if (config == null)
            {
                config = GetDefaultConfig(typeName);
            }
            FillEntityInfo(config);

            return config;
        }

        [IgnoreWebApiMethod]
        public async Task<Type> GetDynamicIndexTypeAsync(string typeFullName, bool withCache = true)
        {
            var config = await GetDynamicIndexConfigAsync(typeFullName);
            if (config == null)
            {
                return null;
            }
            if (withCache)
            {
                if (_typesCache.ContainsKey(typeFullName))
                {
                    return _typesCache[typeFullName];
                }
            }
            var indexType = await GetDynamicIndexTypeAsync(config.EntityInfo);
            _typesCache[typeFullName] = indexType;
            return indexType;
        }

        [IgnoreWebApiMethod]
        public async Task<Type> GetDynamicIndexTypeAsync(DynamicIndexEntityInfo entityInfo)
        {
            IEnumerable<string> usings = new List<string>()
            {
                "EasyOC.Core.Indexes", "FreeSql.DataAnnotations", "EasyOC.OrchardCore.DynamicTypeIndex.Index"
            };

            //Create dynamic classes from the script
            var type = await _cSharpScriptProvider
                .CreateTypeAsync(entityInfo.FullName,
                entityInfo.EntityContent,
                usings
                );
            return type;
        }
        public Task<DynamicIndexConfigModel> GetDynamicIndexConfigAsync(string typeName, bool withCache = false)
        {
            if (typeName.IsNullOrWhiteSpace())
            {
                return null;
            }
            var pkg = _cachedTypeConfigurations.GetOrAdd(typeName,
            async (key) =>
            {
                var contentItem = await YesSession
                    .Query<ContentItem, ContentItemIndex>(x => x.Latest && x.Published)
                    .With<DynamicIndexConfigDataIndex>(x => x.TypeName == typeName)
                    .FirstOrDefaultAsync();
                if (contentItem != null)
                {
                    return ToConfigModel(contentItem);
                }
                return null;
            });

            return pkg;
        }

        private DynamicIndexEntityInfo FillEntityInfo(DynamicIndexConfigModel config)
        {
            var entityInfo = config.EntityInfo;

            // if (config.TypeName.Contains('.'))
            // {
            //     var typeFullName = DefaultNamespace.Split('.').ToList();
            //     typeFullName.AddRange(config.TypeName.Split('.'));
            //     entityInfo.NameSpace = string.Join('.', typeFullName.Take(typeFullName.Count - 1).ToArray());
            //     entityInfo.EntityName = string.Format(DefaultEntityNameTemplate, string.Join('.', typeFullName.Last()));
            // }
            // else
            // {
            entityInfo.NameSpace = DefaultNamespace;
            entityInfo.EntityName = string.Format(DefaultEntityNameTemplate, config.TypeName);
            // }


            var indexCols = config.Fields.Where(x => !x.Disabled && x.IsDefaultIndex)
                .Select(x => x.Name).ToList();
            var indexNames = new List<string>()
            {
                "ContentItemId", "DocumentId", "Published", "Latest"
            };
            if (indexCols.Any())
            {
                indexNames.AddRange(indexCols);
            }

            var fields = new List<string>();
            foreach (var item in config.Fields.Where(x => !x.Disabled))
            {
                var restAttr = new List<string>();

                #region Field Attributes
                if (item.IsPrimaryKey)
                {
                    restAttr.Add("IsPrimary = true");
                }

                if (item.IsIdentity)
                {
                    restAttr.Add("IsIdentity = true");
                }

                if (item.IsNullable)
                {
                    restAttr.Add("IsNullable = true");
                }

                if (item.Length != 0)
                {
                    restAttr.Add("StringLength = " + item.Length);
                }

                var restAttrText = "";
                if (restAttr.Count > 0)
                {
                    restAttrText = "," + restAttr.JoinAsString(",");
                }
                #endregion

                fields.Add($@"
        [Column(Name = ""{item.Name}""{restAttrText})]
        public {ConvertTypeName(item.CsTypeName)} {item.GetCsFieldName()} {{ get; set; }}
                ");
            }


            // ReSharper disable once StringLiteralTypo
            var template = $@"
using EasyOC.Core.Indexes;
using FreeSql.DataAnnotations;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
// 此代码由程序生成，复制到代码文件后请更新命名空间，
// 或者在命名空间处点击 Alt+Enter 自动更新命名空间
namespace {entityInfo.NameSpace}
{{
    [EOCIndex(""IDX_{{tablename}}_DocumentId"",""{string.Join(",", indexNames)}"")]
    [EOCTable(Name = ""{config.TableName}"")]
    public class {entityInfo.EntityName} : DIndexBase
    {{

{fields.JoinAsString(string.Empty)}
    }}
}}
";
            entityInfo.EntityContent = template;
            Console.WriteLine(template);

            return entityInfo;
        }

        [IgnoreWebApiMethod]
        public async Task<Type> SyncTableStructAsync(DynamicIndexEntityInfo entityInfo)
        {
            var type = await GetDynamicIndexTypeAsync(entityInfo);
            //Sync Table Structure
            Fsql.CodeFirst.SyncStructure(type);
            return type;
        }


        [HttpPost]
        public async Task<DynamicIndexConfigModel> UpdateDynamicIndexAsync(DynamicIndexConfigModel model)
        {
            FillEntityInfo(model);
            await SyncTableStructAsync(model.EntityInfo);
            var doc = await YesSession.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.Latest & x.Published)
                .With<DynamicIndexConfigDataIndex>()
                .Where(x => x.TypeName == model.TypeName)
                .FirstOrDefaultAsync();
            var isCreate = false;
            if (doc == null)
            {
                isCreate = true;
                doc = await ContentManager.NewAsync(nameof(DynamicIndexConfigSetting));
            }

            doc.Alter<DynamicIndexConfigSetting>(part =>
            {
                part.TableName = new TextField
                {
                    Text = model.TableName
                };
                part.TypeName = new TextField
                {
                    Text = model.TypeName
                };
                part.ConfigData = new TextField
                {
                    Text = JsonConvert.SerializeObject(model.Fields)
                };
                part.EntityInfo = new TextField
                {
                    Text = JsonConvert.SerializeObject(model.EntityInfo)
                };
            });
            var sucessed = await ContentManager.CreateOrUpdateAndPublishAsync(doc, isCreate,
            new PublishOptions
            {
                HtmlLocalizer = H, Notifier = Notifier
            });

            if (sucessed)
            {
                model.ContentItemId = doc.ContentItemId;
                await YesSession.SaveChangesAsync();
                await RebuildIndexData(model);
                _cachedTypeConfigurations[model.TypeName] = Task.FromResult(model);
                return model;
            }
            else
            {
                return null;
            }
        }


        [HttpGet]
        public async Task<int> RebuildIndexData(string typeName)
        {
            var model = await GetDynamicIndexConfigAsync(typeName);
            if (model == null)
            {
                await Notifier.ErrorAsync(H["DynamicIndex Config :{0} is not found", typeName]);
            }

            return await RebuildIndexData(model);
        }

        [IgnoreWebApiMethod]
        public async Task<int> RebuildIndexData(DynamicIndexConfigModel model)
        {
            var docs = YesSession.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.Latest && x.Published && x.ContentType == model.TypeName)
                .OrderBy(x => x.Id);

            var indexTableName = model.TableName;
            int page = 1;
            var totalRows = 0;


            var take100 = await docs.Take(100).ListAsync();
            while (take100.Any())
            {
                var pendingIds = take100.Select(x => x.ContentItemId);
                await  Fsql.Delete<DIndexBase>()
                    .AsTable(indexTableName)
                    .Where(x => pendingIds.Contains(x.ContentItemId))
                    .ExecuteAffrowsAsync();
                var freeModels = take100.ToDictModel(model);
                // Specifies the collection of data dictionary objects to insert
                var freeItems = Fsql.InsertOrUpdateDict(freeModels)
                    .AsTable(indexTableName)//Specify the name of the table to be inserted
                    .WherePrimary("Id");
                totalRows += await freeItems.ExecuteAffrowsAsync();//Batch Inserting databases
                page++;
                take100 = docs.Skip(page * 100).Take(100).ListAsync().GetAwaiter().GetResult();
            }


            return totalRows;
        }

        private string[] ExceptFields = new[]
        {
            "GeoPointField", "MediaField"
        };

        [IgnoreWebApiMethod]
        public DynamicIndexConfigModel GetDefaultConfig(string typeName)
        {
            var config = new DynamicIndexConfigModel()
            {
                TypeName = typeName, TableName = GetDynamicIndexTableName(typeName)
            };
            var typeDef = ContentDefinitionManager.GetTypeDefinition(typeName);
            if (typeDef is null)
            {
                return null;
            }

            var fields = new List<DynamicIndexFieldItem>();

            //fields.AddDbField<int>("Id", isSystem: true, isPrimaryKey: true, isIdentity: true, isNullable: false, addToTableIndex: true);
            //fields.AddDbField<int>("DocumentId", isSystem: true, addToTableIndex: true);

            foreach (var part in typeDef.Parts)
            {
                foreach (var field in part.PartDefinition.Fields)
                {
                    if (!ExceptFields.Contains(field.FieldDefinition.Name))
                    {
                        fields.Add(field.ToDynamicIndexField(part));
                    }
                }
            }

            config.Fields = fields;

            FillEntityInfo(config);
            return config;
        }


        [IgnoreWebApiMethod]
        public DynamicIndexConfigModel ToConfigModel(ContentItem storedConfig)
        {
            var part = storedConfig.As<DynamicIndexConfigSetting>();
            if (part == null)
            {
                return null;
            }

            var config = new DynamicIndexConfigModel();
            config.TableName = part.TableName.Text;
            config.TypeName = part.TypeName.Text;
            if (!part.ConfigData.Text.IsNullOrWhiteSpace())
            {
                config.Fields = JsonConvert.DeserializeObject<List<DynamicIndexFieldItem>>(part.ConfigData.Text);
            }

            if (part.EntityInfo != null && !part.EntityInfo.Text.IsNullOrWhiteSpace())
            {
                config.EntityInfo = JsonConvert.DeserializeObject<DynamicIndexEntityInfo>(part.EntityInfo.Text);
            }

            config.ContentItemId = storedConfig.ContentItemId;
            return config;
        }

        private string ConvertTypeName(string csTypeName)
        {
            switch (csTypeName)
            {
                case "System.String":
                    return "string";
                case "System.Int32":
                    return "int";
                case "System.Int64":
                    return "long";
                case "System.Decimal":
                    return "decimal";
                default:
                    return csTypeName;
            }
        }

        [IgnoreWebApiMethod]
        public async Task<AssemblyCSharpBuilder> GetIndexAssemblyBuilder(bool withOutCache = false)
        {
            var docs = await YesSession.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.Latest & x.Published)
                .With<DynamicIndexConfigDataIndex>()
                .Where(x => true)
                .ListAsync();

            var typeDefs = new List<string>();
            var typesDict = new Dictionary<string, Type>();
            foreach (var config in docs)
            {
                var model = ToConfigModel(config);
                typesDict.Add(model.EntityInfo.FullName, null);
                typeDefs.Add(model.EntityInfo.EntityContent);
            }

            var builder = await _cSharpScriptProvider.GetAssemblyCSharpBuilderAsync(withOutCache);

            HashSet<string> usings = new HashSet<string>();
            usings.Add("EasyOC.Core.Indexes");
            usings.Add("FreeSql.DataAnnotations");
            usings.Add("EasyOC.OrchardCore.DynamicTypeIndex.Index");
            builder.Domain.UsingRecorder.Using(usings);
            builder.Add(string.Join("\r\n", typeDefs));
            // var asm = builder.GetAssembly();
            // foreach (var key in typesDict.Keys)
            // {
            //     var type = builder.GetTypeFromFullName(key);
            //     typesDict[key] = type;
            // }


            return builder;
        }
    }
}
