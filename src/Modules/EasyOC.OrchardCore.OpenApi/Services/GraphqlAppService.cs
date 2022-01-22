using EasyOC.Core.Application;
using GraphQL.Types;
using OrchardCore.Apis.GraphQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Services
{
    public class GraphqlAppService : AppServcieBase
    {
        private readonly ISchemaFactory _schemaFactory;

        public GraphqlAppService(ISchemaFactory schemaFactory)
        {
            _schemaFactory = schemaFactory;
        }

        public async Task<ISchema> GetSchemaAsync()
        {
            return await _schemaFactory.GetSchemaAsync();
        }
    }
}
