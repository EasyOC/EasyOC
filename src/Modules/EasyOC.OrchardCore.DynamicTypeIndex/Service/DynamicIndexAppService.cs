using EasyOC.Core.Application;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Service.Dto;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex
{
    public class DynamicIndexAppService : AppServiceBase
    {
        public const string DefaultTableNameTemplate = "{0}_DIndex";
        public async Task<DynamicIndexConfigModel> GetDynamicIndexConfigAsync(string typeName)
        {
            if (typeName.IsNullOrWhiteSpace())
            {
                return null;
            }

            var config = await FreeSqlSession.Select<DynamicIndexConfigDataIndex, ContentItemIndex>()
                .InnerJoin<ContentItemIndex>((di, ci) => di.DocumentId == ci.DocumentId && ci.Latest)
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

        [NonDynamicMethod]
        public DynamicIndexConfigModel GetDefaultConfig(string typeName)
        {
            var config = new DynamicIndexConfigModel()
            {
                TypeName = typeName,
                TableName = string.Format(DefaultTableNameTemplate, typeName),
                ConfigDataOptions = new DynamicIndexConfigContent
                {
                    Fields = GenerateFields(typeName)
                }

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

            fields.AddDbField<int>("Id", isSystem: true, isPrimaryKey: true, isIdentity: true, isNullable: false,addToTableIndex:true);
            fields.AddDbField<int>("DocumentId", isSystem: true, addToTableIndex: true);
            fields.AddDbField<string>("ContentItemId", 26, true, addToTableIndex: true);

            foreach (var part in typeDef.Parts)
            {
                foreach (var field in part.PartDefinition.Fields)
                {
                    fields.Add(field.ToDynamicIndexField(part));
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

            if (storedConfig.ConfigData is null)
            {
                config.ConfigDataOptions = new DynamicIndexConfigContent();
            }
            else
            {
                try
                {
                    config.ConfigDataOptions = JsonConvert.DeserializeObject<DynamicIndexConfigContent>(storedConfig.ConfigData);
                }
                catch
                {
                    config.ConfigDataOptions = new DynamicIndexConfigContent();
                }
            }

            var newConfigFields = GenerateFields(config.TypeName);
            if (config.ConfigDataOptions.Fields.Count > 0)
            {
                config.ConfigDataOptions.Fields = Merge(config.ConfigDataOptions.Fields, newConfigFields);
            }
            else
            {
                config.ConfigDataOptions.Fields = newConfigFields;
            }
            return config;
        }


        private List<DynamicIndexFieldItem> Merge(List<DynamicIndexFieldItem> stored, List<DynamicIndexFieldItem> newFields)
        {
            return newFields;
        }






    }
}
