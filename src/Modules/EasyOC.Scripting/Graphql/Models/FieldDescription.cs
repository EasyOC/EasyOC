using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace EasyOC.Scripting.Graphql.Models
{
    public class FieldSchemaDescription
    {
        public FieldSchemaDescription(JProperty fieldSchema)
        {
            Schema = fieldSchema;
            FieldName = fieldSchema.Name.Replace('.', '_');
            ValuePath = fieldSchema.Name;
        }
        public static FieldSchemaDescription Parse(JProperty fieldSchema)
        {
            var fieldDescription = new FieldSchemaDescription(fieldSchema);

            // items:{"type":{}}
            // items:{"name":"int"}
            if (fieldSchema.Value.Type == JTokenType.Object)
            {
                var schemaValue = fieldSchema.Value;
                //包含Type属性
                if (((JObject)schemaValue).ContainsKey("type"))
                {
                    fieldDescription.Description = schemaValue["description"]?.Value<string>();
                    switch (schemaValue["type"]?.Type)
                    {
                        case JTokenType.Object:
                            // items:{type:{}}
                            /* 保存在Type 中的 对象
            {
                "items": {
                    "type": {
                        "name": "string",
                        "age": "int",
                        "sex": "boolean",                
                        "customers": "Customer",
                        "customerOrders": "Order"
                    },
                    "description":"dddddddddddd"
                }
            }
         */
                            fieldDescription.TypeName = "object";
                            foreach (var jToken in schemaValue["type"])
                            {
                                var prop = (JProperty)jToken;
                                fieldDescription.Fields.Add(Parse(prop));
                            }
                            return fieldDescription;

                        case JTokenType.String:
                            // items:{type:"int",description:"123124"}
                            /* 类型保存在 Type中的 字符串
            {
                "items":{"type": "int","description":"dddddddddddd"},        
                "items":{"type": "Customer"}
            }
         */
                            fieldDescription.TypeName = schemaValue["type"]?.Value<string>();

                            if (fieldDescription.TypeName == "array")
                            {
                                fieldDescription.OfType = "object";
                                if (schemaValue["ofType"].Type == JTokenType.Object)
                                {
                                    foreach (var jToken in schemaValue["ofType"])
                                    {
                                        var prop = (JProperty)jToken;
                                        fieldDescription.Fields.Add(Parse(prop));
                                    }
                                }
                                else
                                {
                                    var ofType = schemaValue["ofType"]?.Value<string>();
                                    if (ofType != null)
                                        fieldDescription.OfType = schemaValue["ofType"]?.Value<string>();
                                }
                            }
                            return fieldDescription;
                    }
                }
                //不包含 Type 属性
                // items:{ customers:"Customer",users:"User",userId:"int" }
                foreach (var jToken in fieldSchema.Value)
                {
                    var prop = (JProperty)jToken;
                    fieldDescription.Fields.Add(Parse(prop));
                }
                return fieldDescription;
            }
            /* 直接类型
{
    "items":{
        "name": "string",
        "age": "int",
        "sex": "boolean",
        "customers": "Customer"
    }
}
 */
            // items:"int"  items:"Customer"  
            fieldDescription.TypeName = fieldSchema.Value.ToString();
            return fieldDescription;
        }

        public List<FieldSchemaDescription> Fields { get; set; } = new List<FieldSchemaDescription>();

        public JProperty Schema { get; set; }
        [JsonProperty("type")]
        public string TypeName { get; set; }
        [JsonProperty("ofType")]
        public string OfType { get; set; }
        [JsonProperty("name")]
        public string FieldName { get; set; }
        public string ValuePath { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}