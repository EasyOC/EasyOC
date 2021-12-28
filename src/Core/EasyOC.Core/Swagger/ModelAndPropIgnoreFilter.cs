using EasyOC.Core.Swagger.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace EasyOC.Core.Swagger
{
    public class ModelAndPropIgnoreFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;

            if (schema?.Properties == null || type == null)
                return;
            var excludedProperties = type.GetProperties()
                                         .Where(t => t.GetCustomAttribute<SwaggerIgnoreAttribute>() != null);

            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name))
                    schema.Properties.Remove(excludedProperty.Name);
            }
        }
    }
}
