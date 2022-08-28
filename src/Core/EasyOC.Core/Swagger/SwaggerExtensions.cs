using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Linq;
using System.Text;

namespace EasyOC.Core.Swagger
{
    public static class SwaggerExtensions
    {   
        /// <summary>
        /// https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/752#issuecomment-467817189
        /// When Swashbuckle.AspNetCore 5.0 is released, we can remove it.
        /// </summary>
        /// <param name="options"></param>
        public static void CustomDefaultSchemaIdSelector(this SwaggerGenOptions options)
        {
            string SchemaIdSelector(Type modelType)
            {
                if (!modelType.IsConstructedGenericType)
                {
                    return modelType.Name;
                }

                var prefix = modelType.GetGenericArguments()
                    .Select(SchemaIdSelector)
                    .Aggregate((previous, current) => previous + current);

                return modelType.Name.Split('`').First() + "Of" + prefix;
            }

            options.CustomSchemaIds(SchemaIdSelector);
        }
    }
}
