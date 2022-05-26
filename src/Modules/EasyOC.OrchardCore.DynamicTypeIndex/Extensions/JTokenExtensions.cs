using Newtonsoft.Json.Linq;
using EasyOC.OrchardCore.DynamicTypeIndex.Models;
using OrchardCore.ContentFields.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Linq
{
    public static class JTokenExtentions
    {
        public static object GetValue(this JToken jToken)
        {
            object value = null;
            switch (jToken.Type)
            {
                case JTokenType.None:
                    break;
                case JTokenType.Object:
                    value = jToken.Value<object>();
                    break;
                case JTokenType.Array:
                    value = jToken.Value<Array>();
                    break;
                case JTokenType.Constructor:
                    break;
                case JTokenType.Property:
                    break;
                case JTokenType.Comment:
                    break;
                case JTokenType.Integer:
                    value = jToken.Value<int>();

                    break;
                case JTokenType.Float:
                    value = jToken.Value<float>();

                    break;
                case JTokenType.String:
                    value = jToken.Value<string>();

                    break;
                case JTokenType.Boolean:
                    value = jToken.Value<bool?>();
                    break;
                case JTokenType.Null:
                    break;
                case JTokenType.Undefined:
                    break;
                case JTokenType.Date:
                    value = jToken.Value<DateTime?>();
                    break;
                case JTokenType.Raw:
                    break;
                case JTokenType.Bytes:
                    value = jToken.Value<byte?>();
                    break;
                case JTokenType.Guid:
                    value = jToken.Value<Guid?>();

                    break;
                case JTokenType.Uri:
                    value = jToken.Value<string>();
                    break;
                case JTokenType.TimeSpan:
                    value = jToken.Value<TimeSpan?>();
                    break;
                default:
                    value = (object)jToken;
                    break;
            }
            return value;
        }

        public static object GetOCFieldValue(this JToken jToken,ContentFieldOption config)
        {

            switch (config.FieldType)
            {
                case nameof(TextField):
                case nameof(LinkField):
                case nameof(HtmlField):
                    return jToken.Value<string>();
                case nameof(ContentPickerField):
                case nameof(UserPickerField):
                    if (jToken.Type == JTokenType.Array)
                    {
                        return jToken.FirstOrDefault()?.Value<string>();
                    }
                    return jToken?.Value<string>();
                case nameof(BooleanField):
                    return jToken.Value<bool?>();
                case nameof(NumericField):
                    return jToken.Value<decimal?>();
                case nameof(DateTimeField):
                    return jToken.Value<DateTime?>();
                case nameof(DateField):
                    return jToken.Value<DateTime?>();
                case nameof(TimeField):
                    return jToken?.Value<string>();


            }
            return jToken.Value<string>();
        }

    }
}
