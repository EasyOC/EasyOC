using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace EasyOC.Core.Swagger
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Injects ABP base URI into the index.html page
        /// </summary>
        /// <param name="options"></param>
        /// <param name="pathBase">base path (URL) to application API</param>
        public static void InjectBaseUrl(this SwaggerUIOptions options, string pathBase)
        {
            pathBase = pathBase.EnsureEndsWith('/');

            options.HeadContent = new StringBuilder(options.HeadContent)
                .AppendLine($"<script> var abp = abp || {{}}; abp.appPath = abp.appPath || '{pathBase}'; </script>")
                .ToString();
        }

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
