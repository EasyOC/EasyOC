using Newtonsoft.Json.Linq;
using System.Diagnostics.Contracts;
using System.Linq;

namespace EasyOC.Core.Extensions
{
    // https://andrewlock.net/serializing-a-pascalcase-newtonsoft-json-jobject-to-camelcase/
    // TODO Find better approach.
    public static class JTokenExtensions
    {
        // Recursively converts a JObject with PascalCase names to camelCase
        [Pure]
        public static JObject ToPascalCase(this JObject original)
        {
            var newObj = new JObject();
            foreach (var property in original.Properties())
            {
                var newPropertyName = property.Name.ToPascalCaseString();
                newObj[newPropertyName] = property.Value.ToPascalCaseJToken();
            }

            return newObj;
        }

        // Recursively converts a JToken with camelCase names to PascalCase
        [Pure]
        public static JToken ToPascalCaseJToken(this JToken original)
        {
            switch (original.Type)
            {
                case JTokenType.Object:
                    return ((JObject)original).ToPascalCase();
                case JTokenType.Array:
                    return new JArray(((JArray)original).Select(x => x.ToPascalCaseJToken()));
                default:
                    return original.DeepClone();
            }
        }

        // Convert a string to camelCase
        [Pure]
        public static string ToPascalCaseString(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return char.ToUpperInvariant(str[0]) + str.Substring(1);
            }

            return str;
        }
    }
}



