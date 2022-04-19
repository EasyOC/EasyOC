using EasyOC.Core.Application;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Reflection;
using System.Threading.Tasks;
using YesSql;
using YesSql.Sql;
using YesSql.Sql.Schema;

namespace EasyOC.OrchardCore.DynamicTypeIndex
{
    public class DynamicIndexAppService : AppServiceBase, IDynamicIndexAppService
    {
        public const string DefaultTableNameTemplate = "{0}{1}_DIndex";
        private static object indexConfigCacheLocker = new object();
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<string, Task<DynamicIndexConfigModel>> _cachedTypeConfigurations;
        private readonly IStore _store;
        private readonly IShellConfiguration _shellConfiguration;
        private string GetDynamicIndexTableName(string typeName)
        {
            var shellConfig = ShellScope.Current.ServiceProvider.GetRequiredService<IShellConfiguration>();
            var dbOptions = shellConfig.Get<DatabaseShellsStorageOptions>();
            return string.Format(DefaultTableNameTemplate, dbOptions.TablePrefix, typeName);
        }
        public DynamicIndexAppService(IStore store, IShellConfiguration shellConfiguration, IMemoryCache memoryCache)
        {
            _store = store;
            _shellConfiguration = shellConfiguration;
            _memoryCache = memoryCache;
            //_cachedTypeConfigurations = _memoryCache.GetOrCreate("CachedTypeConfigurations", entry => new ConcurrentDictionary<string, DynamicIndexCachePakage>());
            _cachedTypeConfigurations = _memoryCache.GetOrCreate("CachedTypeConfigurations",
                entry => new ConcurrentDictionary<string, Task<DynamicIndexConfigModel>>());

        }
        public async Task<DynamicIndexConfigModel> GetDynamicIndexConfigOrDefaultAsync(string typeName)
        {
            var config = await GetDynamicIndexConfigAsync(typeName);
            if (config == null)
            {
                config = GetDefaultConfig(typeName);
            }
            return config;
        }
        
        public Task<DynamicIndexConfigModel> GetDynamicIndexConfigAsync(string typeName)
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
                    else
                    {
                        return null;
                    }
                    //var configPkg = new DynamicIndexCachePakage()
                    //{
                    //    Document = contentItem,
                    //    ConfigModel = configModel
                    //};
                    //return configPkg;

                });
            return pkg;
        }

        private DynamicIndexEntityInfo FillEntityInfo(DynamicIndexConfigModel config)
        {
            var entityInfo = config.EntityInfo;

            entityInfo.EntityName = config.TableName.Replace("_", string.Empty);
            var indexCols = config.Fields.
                Where(x => !x.Disabled && x.IsDefaultIndex)
                .Select(x => x.Name).ToList();
            var INDEX_NAMES = new[] { "DocumentId", "ContentItemId" }.ToList();
            if (indexCols.Any())
            {
                INDEX_NAMES.AddRange(indexCols);
            }
            var fileds = new List<string>();
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

                fileds.Add($@"
        [Column(Name = ""{item.Name}""{restAttrText})]
        public {item.CsTypeName} {item.Name.Replace("_", string.Empty)} {{ get; set; }} 
                ");
            }

            var FIELDS = fileds.JoinAsString(string.Empty);

            entityInfo.NameSpace = "EasyOC.DynamicTypeIndex.IndexModels";

            var template = $@"
using EasyOC.Core.Indexs;
using FreeSql.DataAnnotations;
namespace {entityInfo.NameSpace}
{{
    [EOCIndex(""IDX_{{tablename}}_DocumentId"",""{ string.Join(",", INDEX_NAMES)}"")]
    [EOCTable(Name = ""{config.TableName}"")]
    public class { entityInfo.EntityName } : FreeSqlDocumentIndex
    {{
        [Column(StringLength = 26)]
        public string ContentItemId {{ get; set; }}
{FIELDS}
    }}
}}
";
            entityInfo.EntityContent = template;
            Console.WriteLine(template);

            return entityInfo;
        }
        [NonDynamicMethod]
        public Type SyncTableStructAsync(DynamicIndexEntityInfo entityInfo)
        {
            //Create dynamic classes from the script
            AssemblyCSharpBuilder oop = new AssemblyCSharpBuilder();
            //Even if you add 100 classes, they will all be compiled in one assembly
            oop.Add(entityInfo.EntityContent);
            Assembly asm = oop.GetAssembly();
            var type = asm.GetType(entityInfo.FullName);
            //Sync Table Structure
            Fsql.CodeFirst.SyncStructure(type);
            return type;
        }


        [HttpPost]
        public async Task<DynamicIndexConfigModel> UpdateDynamicIndexAsync(DynamicIndexConfigModel model)
        {
            FillEntityInfo(model);
            SyncTableStructAsync(model.EntityInfo);
            var doc = await YesSession.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.Latest & x.Published)
                .With<DynamicIndexConfigDataIndex>()
                .Where(x => x.TypeName == model.TypeName)
                .FirstOrDefaultAsync();
            if (doc == null)
            {
                doc = await ContentManager.NewAsync(nameof(DynamicIndexConfigSetting));
            }
            doc.Alter<DynamicIndexConfigSetting>(part =>
            {
                part.TableName = new TextField { Text = model.TableName };
                part.TypeName = new TextField { Text = model.TypeName };
                part.ConfigData = new TextField { Text = JsonConvert.SerializeObject(model.Fields) };
                part.EntityInfo = new TextField { Text = JsonConvert.SerializeObject(model.EntityInfo) };
            });
            var result = await ContentManager.ValidateAsync(doc);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    await Notifier.ErrorAsync(H[error.ErrorMessage, error.MemberNames]);
                }
                return null;
            }

            await ContentManager.PublishAsync(doc);
            model.ContentItemId = doc.ContentItemId;
            await RebuildIndexData(model);
            _cachedTypeConfigurations[model.TypeName] = Task.FromResult(model);
            return model;
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

        [NonDynamicMethod]
        public async Task<int> RebuildIndexData(DynamicIndexConfigModel model)
        {
            var docs = YesSession.Query<ContentItem, ContentItemIndex>()
               .Where(x => x.Latest && x.Latest && x.ContentType == model.TypeName)
               .OrderBy(x => x.Id);

            var indexTableName = model.TableName;
            var take100 = await docs.Take(100).ListAsync();
            int page = 1;
            var totalRows = 0;
            Fsql.Delete<object>()
                   .AsTable(indexTableName)
                   .Where("1=1")
                   .ExecuteAffrows();

            while (take100.Any())
            {
                var freeModels = take100.ToDictModel(model);
                // Specifies the collection of data dictionary objects to insert
                var freeItems = Fsql.InsertDict(freeModels)
                     .AsTable(indexTableName);  //Specify the name of the table to be inserted
                totalRows += await freeItems.ExecuteAffrowsAsync();//Batch Inserting databases
                page++;
                take100 = await docs.Skip(page * 100).Take(100).ListAsync();
            }

            return totalRows;
        }

        private string[] ExceptFields = new[] { "GeoPointField" };
        [NonDynamicMethod]
        public DynamicIndexConfigModel GetDefaultConfig(string typeName)
        {
            var config = new DynamicIndexConfigModel()
            {
                TypeName = typeName,
                TableName = GetDynamicIndexTableName(typeName)
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



        [NonDynamicMethod]
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



        //public async Task SyncTableStructAsync(DynamicIndexConfigModel model, bool dropTable = false)
        //{
        //    var schemaBuilder = new SchemaBuilder(_store.Configuration, await YesSession.BeginTransactionAsync(), true);
        //    var indexTableName = schemaBuilder.GetIndexTableName(model.TableName);
        //    var table = Fsql.DbFirst.GetTableByName(indexTableName);
        //    if (table != null)
        //    {
        //        //if (dropTable)
        //        //{
        //        //    await Notifier.AddAsync(NotifyType.Warning, H["数据表已存在，继续执行将删除旧表，请通过重建索引功能拉回数据"]);
        //        //    return;
        //        //}
        //        schemaBuilder.DropTable(model.TableName);
        //        //schemaBuilder.Transaction.Commit();
        //        await Notifier.AddAsync(NotifyType.Warning, H["The index table {0}，has been dropped .", indexTableName]);
        //    }
        //    schemaBuilder.CreateMapIndexTable(model.TableName, cmd =>
        //    {
        //        cmd.Column<string>("ContentItemId", yesCol =>
        //        {
        //            yesCol.WithLength(26);

        //        });
        //        var filterd = model.Fields.Where(x => x != null &&
        //         !x.Name.IsNullOrEmpty() &&
        //         !x.CsTypeName.IsNullOrEmpty());
        //        foreach (var dbOption in filterd)
        //        {

        //            cmd.Column(dbOption.Name, Type.GetType(dbOption.CsTypeName),
        //                yesCol =>
        //                {
        //                    if (dbOption.IsIdentity)
        //                    {
        //                        yesCol.Identity();
        //                    }
        //                    if (dbOption.IsPrimaryKey)
        //                    {
        //                        yesCol.PrimaryKey();
        //                    }
        //                    if (dbOption.Length > 0)
        //                    {
        //                        yesCol.WithLength(dbOption.Length);
        //                    }
        //                    if (dbOption.IsNullable)
        //                    {
        //                        yesCol.Nullable();
        //                    }
        //                }
        //                );
        //        }
        //    });
        //    schemaBuilder.Transaction.Commit();
        //}



    }
}
