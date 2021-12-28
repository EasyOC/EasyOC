using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC.Core.Swagger
{
    public class SwaggerEnumParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var type = Nullable.GetUnderlyingType(context.ApiParameterDescription.Type) ?? context.ApiParameterDescription.Type;

            if (type.IsEnum)
            {
                AddEnumParamSpec(parameter, type, context);
                parameter.Required = type == context.ApiParameterDescription.Type;
            }
            else if (type.IsArray || type.IsGenericType && type.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                var itemType = type.GetElementType() ?? type.GenericTypeArguments.First();
                AddEnumSpec(itemType, context);
            }

        }

        private static void AddEnumSpec(Type type, ParameterFilterContext context)
        {
            var schema = context.SchemaRepository.Schemas.GetOrAdd($"#/definitions/{type.Name}", () =>
                context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository)
            );

            if (schema.Reference == null || !type.IsEnum)
            {
                return;
            }

            var enumNames = new OpenApiArray();
            enumNames.AddRange(Enum.GetNames(type).Select(_ => new OpenApiString(_)));
            schema.Extensions.Add("x-enumNames", enumNames);
        }

        private static void AddEnumParamSpec(OpenApiParameter parameter, Type type, ParameterFilterContext context)
        {
            var schema = context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
            if (schema.Reference == null)
            {
                return;
            }

            parameter.Schema = schema;

            var enumNames = new OpenApiArray();
            enumNames.AddRange(Enum.GetNames(type).Select(_ => new OpenApiString(_)));
            schema.Extensions.Add("x-enumNames", enumNames);
        }
    }
}
