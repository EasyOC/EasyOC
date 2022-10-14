using EasyOC.Core.Application;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.GraphQL.Models;
using GraphQL;
using GraphQL.Execution;
using GraphQL.SystemTextJson;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OrchardCore.Apis.GraphQL;
using OrchardCore.DisplayManagement.Notify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasyOC.GraphQL.Servicies
{
    public class GraphqlExecuterService : AppServiceBase, IGraphqlExecuterService
    {
        private readonly GraphQLSettings _settings;
        private readonly ISchemaFactory _schemaService;
        internal static readonly Encoding _utf8Encoding = new UTF8Encoding(false);
        private readonly IDocumentExecuter _executer;

        public GraphqlExecuterService(ISchemaFactory schemaService, IDocumentExecuter executer, GraphQLSettings settings)
        {
            _schemaService = schemaService;
            _executer = executer;
            _settings = settings;
        }

        [IgnoreWebApiMethod]
        public async Task<ExecutionResult> ExecuteQuery(GraphQLRequest request)
        {
            var context = HttpContextAccessor.HttpContext;
            var schema = await _schemaService.GetSchemaAsync();


            var dataLoaderDocumentListener = context.RequestServices.GetRequiredService<IDocumentExecutionListener>();
            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = request.Query;
                _.Inputs = request.Variables.ToInputs();
                _.UserContext = _settings.BuildUserContext?.Invoke(context);
                _.ThrowOnUnhandledException = _settings.ExposeExceptions;
                _.ValidationRules = DocumentValidator.CoreRules
                                        .Concat(context.RequestServices.GetServices<IValidationRule>());
                _.ComplexityConfiguration = new ComplexityConfiguration
                {
                    MaxDepth = _settings.MaxDepth,
                    MaxComplexity = _settings.MaxComplexity,
                    FieldImpact = _settings.FieldImpact
                };
                _.Listeners.Add(dataLoaderDocumentListener);
                _.RequestServices = context.RequestServices;
            });
            ;

            return result;
        }

    }
}
