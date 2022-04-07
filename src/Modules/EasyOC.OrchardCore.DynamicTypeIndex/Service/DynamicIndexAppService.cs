using EasyOC.Core.Application;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Service.Dto;
using Microsoft.AspNetCore.Mvc;
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
        public const string DefaultTableNameTemplate = "{0}_DIndex";

        private readonly IStore _store;
        private readonly IShellConfiguration _shellConfiguration;
        private string GetDynamicIndexTableName(string typeName)
        {
            return string.Format(DefaultTableNameTemplate, typeName);
        }
        public DynamicIndexAppService(IStore store, IShellConfiguration shellConfiguration)
        {
            _store = store;
            _shellConfiguration = shellConfiguration;
        }

        public async Task<DynamicIndexConfigModel> GetDynamicIndexConfigAsync(string typeName)
        {
            if (typeName.IsNullOrWhiteSpace())
            {
                return null;
            }

            var config = await FreeSqlSession.Select<DynamicIndexConfigDataIndex, ContentItemIndex>()
                .InnerJoin<ContentItemIndex>((di, ci) => di.ContentItemId == ci.ContentItemId && ci.Latest)
                .Where((di, ci) => di.TypeName == typeName)
                .FirstAsync();

            if (config == null)
            {
                return GetDefaultConfig(typeName);

            }
            else
            {
                return ToConfigModel(config);
            }
        }

        public async Task SyncTableStructAsync(DynamicIndexConfigModel model, bool dropTable = false)
        {
            var schemaBuilder = new SchemaBuilder(_store.Configuration, await YesSession.BeginTransactionAsync(), true);
            var indexTableName = schemaBuilder.GetIndexTableName(model.TableName);
            var table = FreeSqlSession.DbFirst.GetTableByName(indexTableName);
            if (table != null)
            {
                //if (dropTable)
                //{
                //    await Notifier.AddAsync(NotifyType.Warning, H["数据表已存在，继续执行将删除旧表，请通过重建索引功能拉回数据"]);
                //    return;
                //}
                schemaBuilder.DropTable(model.TableName);
                //schemaBuilder.Transaction.Commit();
                await Notifier.AddAsync(NotifyType.Warning, H["The index table {0}，has been dropped .", indexTableName]);
            }

            #region 更新表，因为数据不会丢失，可以通过重建索引拿回数据
            //_schemaBuilder.AlterTable(model.TableName, alter =>
            //{
            //    foreach (var fieldItem in model.ConfigDataOptions.Fields)
            //    {
            //        var dbOption = fieldItem.DbFieldOption;
            //        var col = table.Columns.FirstOrDefault(x => x.Name == fieldItem.DbFieldOption.Name);
            //        if (col != null)
            //        {
            //            //更改字段类型
            //            alter.AlterColumn(dbOption.Name, yesCol =>
            //            {
            //                if (dbOption.Length > 0 && col.MaxLength != dbOption.Length)
            //                {
            //                    yesCol.WithLength(dbOption.Length);
            //                }
            //            });
            //        }
            //        else
            //        {

            //        }
            //    }
            //    var deletedCols=
            //});
            #endregion

            schemaBuilder.CreateMapIndexTable(model.TableName, cmd =>
            {
                cmd.Column<string>("ContentItemId", yesCol =>
                {
                    yesCol.WithLength(26);

                });
                var filterd = model.Fields.Where(x => x.DbFieldOption != null &&
                 !x.DbFieldOption.Name.IsNullOrEmpty() &&
                 !x.DbFieldOption.CsTypeName.IsNullOrEmpty());
                foreach (var fieldItem in filterd)
                {

                    var dbOption = fieldItem.DbFieldOption;
                    cmd.Column(dbOption.Name, Type.GetType(dbOption.CsTypeName),
                        yesCol =>
                        {
                            if (dbOption.IsIdentity)
                            {
                                yesCol.Identity();
                            }
                            if (dbOption.IsPrimaryKey)
                            {
                                yesCol.PrimaryKey();
                            }
                            if (dbOption.Length > 0)
                            {
                                yesCol.WithLength(dbOption.Length);
                            }
                            if (dbOption.IsNullable)
                            {
                                yesCol.Nullable();
                            }
                        }
                        );
                }
            });
            schemaBuilder.Transaction.Commit();
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="syncStruct">同步表结构</param>
        /// <returns></returns>
        public async Task<string> GetIndexClassString(string typeName, bool syncStruct = false)
        {
            var config = await GetDynamicIndexConfigAsync(typeName);
            return GetIndexClassString(config, syncStruct);
        }

        private string GetIndexClassString(DynamicIndexConfigModel config, bool syncStruct = false)
        {
            var indexCols = config.Fields.
                Where(x => !x.DbFieldOption.Disabled && x.DbFieldOption.AddToTableIndex)
                .Select(x => x.DbFieldOption.Name).ToList();
            var INDEX_NAMES = new[] { "DocumentId", "ContentItemId" }.ToList();
            if (indexCols.Any())
            {
                INDEX_NAMES.AddRange(indexCols);
            }
            var fileds = new List<string>();
            foreach (var item in config.Fields.Where(x => !x.DbFieldOption.Disabled))
            {
                var restAttr = new List<string>();

                #region Field Attributes
                if (item.DbFieldOption.IsPrimaryKey)
                {
                    restAttr.Add("IsPrimary = true");
                }
                if (item.DbFieldOption.IsIdentity)
                {
                    restAttr.Add("IsIdentity = true");
                }
                if (item.DbFieldOption.IsNullable)
                {
                    restAttr.Add("IsNullable = true");
                }
                if (item.DbFieldOption.Length != 0)
                {
                    restAttr.Add("StringLength = " + item.DbFieldOption.Length);
                }
                var restAttrText = "";
                if (restAttr.Count > 0)
                {
                    restAttrText = "," + restAttr.JoinAsString(",");
                }
                #endregion

                fileds.Add($@"
                [Column(Name = ""{item.DbFieldOption.Name}""{restAttrText})]
                public {item.DbFieldOption.CsTypeName} {item.DbFieldOption.Name.Replace("_", string.Empty)} {{ get; set; }} 
                ");
            }

            var FIELDS = fileds.JoinAsString("\r\n");

            var className = config.TableName.Replace("_", string.Empty);
            var template = $@"
using EasyOC.Core.Indexs;
using FreeSql.DataAnnotations;
namespace EasyOC.DynamicTypeIndex.IndexModels
{{
    [EOCIndex(""IDX_{{tablename}}_DocumentId"",""{ string.Join(",", INDEX_NAMES)}"")]
    [EOCTable(Name = ""{config.TableName}"")]
    public class {className} : FreeSqlDocumentIndex
    {{
        [Column(StringLength = 26)]
        public string ContentItemId {{ get; set; }}
        {FIELDS}
    }}
}}
";
            if (syncStruct)
            {
                //Create dynamic classes from the script
                AssemblyCSharpBuilder oop = new AssemblyCSharpBuilder();
                //Even if you add 100 classes, they will all be compiled in one assembly
                oop.Add(template);
                Assembly asm = oop.GetAssembly();
                //Sync Table Structure
                FreeSqlSession.CodeFirst.SyncStructure(asm.GetType("EasyOC.DynamicTypeIndex.IndexModels." + className));
            }


            return template;
        }

        [HttpPost]
        public async Task<DynamicIndexConfigModel> UpdateDynamicIndexAsync([FromBody] DynamicIndexConfigModel model)
        {
            var doc = await YesSession.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.Latest)
                .With<DynamicIndexConfigDataIndex>()
                .Where(x => x.TypeName == model.TypeName)
                .FirstOrDefaultAsync();
            if (doc == null)
            {
                doc = await ContentManager.NewAsync(nameof(DynamicIndexConfigSetting));
            }
            var part = doc.As<DynamicIndexConfigSetting>();
            part.ConfigData = new TextField { Text = JsonConvert.SerializeObject(model.Fields) };
            part.TableName = new TextField { Text = model.TableName };
            part.TypeName = new TextField { Text = model.TypeName };
            await ContentManager.PublishAsync(doc);
            await SyncTableStructAsync(model);
            await RebuildIndexData(model);
            return model;
        }


        [HttpGet]
        public async Task<bool> RebuildIndexData(string typeName)
        {
            var model = await GetDynamicIndexConfigAsync(typeName);
            return await RebuildIndexData(model);
        }

        [NonDynamicMethod]
        public async Task<bool> RebuildIndexData(DynamicIndexConfigModel model)
        {
            var shellConfig = ShellScope.Current.ServiceProvider.GetRequiredService<IShellConfiguration>();
            var dbOptions = shellConfig.Get<DatabaseShellsStorageOptions>();
            var docs = YesSession.Query<ContentItem, ContentItemIndex>()
               .Where(x => x.Latest && x.ContentType == model.TypeName)
               .OrderBy(x => x.Id);

            var indexTableName = dbOptions.TablePrefix ?? "" + model.TableName;
            var take100 = await docs.Take(100).ListAsync();
            int page = 1;
            var freeModels = new List<Dictionary<string, object>>();
            var totalRows = 0;
            while (take100.Any())
            {
                foreach (var doc in take100)
                {
                    var jdoc = doc.Content as JObject;
                    var dmodel = new Dictionary<string, object>();
                    dmodel["ContentItemId"] = doc.ContentItemId;
                    dmodel["DocumentId"] = doc.Id;
                    foreach (var fConfig in model.Fields)
                    {
                        JToken valueToken;
                        if (!fConfig.ContentFieldOption.ValueFullPath.IsNullOrWhiteSpace())
                        {
                            valueToken = jdoc.SelectToken(fConfig.ContentFieldOption.ValueFullPath);
                            if (valueToken != null)
                            {
                                dmodel[fConfig.DbFieldOption.Name] = valueToken
                                    .GetOCFieldValue(fConfig.ContentFieldOption.FieldName);
                            }
                        }
                        else
                        {
                            valueToken = jdoc.SelectToken(fConfig.Name);
                            if (valueToken != null)
                            {
                                switch (valueToken.Type)
                                {
                                    case JTokenType.Integer:
                                        dmodel[fConfig.DbFieldOption.Name] = valueToken.Value<int?>();
                                        break;
                                    case JTokenType.Float:
                                        dmodel[fConfig.DbFieldOption.Name] = valueToken.Value<float?>();
                                        break;
                                    case JTokenType.String:
                                        dmodel[fConfig.DbFieldOption.Name] = valueToken.Value<string>();
                                        break;
                                    case JTokenType.Boolean:
                                        dmodel[fConfig.DbFieldOption.Name] = valueToken.Value<bool?>();
                                        break;
                                    case JTokenType.Date:
                                        dmodel[fConfig.DbFieldOption.Name] = valueToken.Value<DateTime?>();
                                        break;
                                    case JTokenType.TimeSpan:
                                        dmodel[fConfig.DbFieldOption.Name] = valueToken.Value<TimeSpan?>();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    freeModels.Add(dmodel);
                }
                // Specifies the collection of data dictionary objects to insert
                var freeItems = FreeSqlSession.InsertDict(freeModels)
                    .AsTable(indexTableName); //Specify the name of the table to be inserted
                totalRows += freeItems.ExecuteAffrows();//Batch Inserting databases
                page++;
                take100 = await docs.Skip(page * 100).Take(100).ListAsync();
            }

            return true;
        }






        private List<DynamicIndexFieldItem> Merge(List<DynamicIndexFieldItem> stored, List<DynamicIndexFieldItem> newFields)
        {
            return newFields;
        }

        private string[] ExceptFields = new[] { "GeoPointField" };
        [NonDynamicMethod]
        public DynamicIndexConfigModel GetDefaultConfig(string typeName)
        {
            var config = new DynamicIndexConfigModel()
            {
                TypeName = typeName,
                TableName = string.Format(DefaultTableNameTemplate, typeName),
                Fields = GenerateFields(typeName)


            };
            return config;
        }
        [NonDynamicMethod]
        public List<DynamicIndexFieldItem> GenerateFields(string typeName)
        {

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
            return fields;
        }



        [NonDynamicMethod]
        public DynamicIndexConfigModel ToConfigModel(DynamicIndexConfigDataIndex storedConfig)
        {
            var config = new DynamicIndexConfigModel()
            {
                TypeName = storedConfig.TypeName,
                TableName = storedConfig.TypeName,
                ContentItemId = storedConfig.ContentItemId,

            };

            try
            {
                config.Fields = JsonConvert.DeserializeObject<List<DynamicIndexFieldItem>>(storedConfig.ConfigData);
            }
            catch
            {
            }

            var newConfigFields = GenerateFields(config.TypeName);
            if (config.Fields.Count > 0)
            {
                config.Fields = Merge(config.Fields, newConfigFields);
            }
            else
            {
                config.Fields = newConfigFields;
            }
            return config;
        }






    }
}
