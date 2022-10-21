using EasyOC.Core.Swagger.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyOC.Core.Swagger
{
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            for (var i = 0; i < operation.Parameters.Count; ++i)
            {
                var parameter = operation.Parameters[i];

                var enumType = context.ApiDescription.ParameterDescriptions[i].ParameterDescriptor.ParameterType;
                if (!enumType.IsEnum)
                {
                    continue;
                }

                var schema = context.SchemaRepository.Schemas.GetOrAdd($"#/definitions/{enumType.Name}", () =>
                    context.SchemaGenerator.GenerateSchema(enumType, context.SchemaRepository)
                );

                parameter.Schema = schema;
            }

            var propertiesToRemove = context.MethodInfo.GetParameters()
            //.Where(methodParm => methodParm.GetCustomAttribute(typeof(FromQueryAttribute), true) != null)
            .SelectMany(methodParm => methodParm.ParameterType.GetProperties())
            .Where(property => property.GetCustomAttribute<SwaggerIgnoreAttribute>() != null)
            .Select(prop => prop.Name) 
            .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            foreach (var parm in operation.Parameters.ToList())
            {
                if (propertiesToRemove.Contains(parm.Name))
                {
                    operation.Parameters.Remove(parm);
                }
            }
        }


    }
}
