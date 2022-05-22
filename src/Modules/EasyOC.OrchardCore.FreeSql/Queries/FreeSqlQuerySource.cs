using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EasyOC.OrchardCore.DynamicTypeIndex.Service;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.Options;
using Natasha.CSharp;
using Natasha.Error;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Data;
using OrchardCore.Liquid;
using OrchardCore.Queries;
using System;
using YesSql;

namespace EasyOC.OrchardCore.FreeSql.Queries
{
    public class FreeSqlQuerySource : IQuerySource
    {
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly IDbConnectionAccessor _dbConnectionAccessor;
        private readonly ISession _session;
        private readonly TemplateOptions _templateOptions;
        private readonly IDynamicIndexAppService _dynamicIndexAppService;

        public FreeSqlQuerySource(
            ILiquidTemplateManager liquidTemplateManager,
            IDbConnectionAccessor dbConnectionAccessor,
            ISession session,
            IOptions<TemplateOptions> templateOptions, IDynamicIndexAppService dynamicIndexAppService)
        {
            _liquidTemplateManager = liquidTemplateManager;
            _dbConnectionAccessor = dbConnectionAccessor;
            _session = session;
            _dynamicIndexAppService = dynamicIndexAppService;
            _templateOptions = templateOptions.Value;
        }

        public string Name => "FreeSql";

        public Query Create()
        {
            return new FreeSqlQuery();
        }


        public async Task<IQueryResults> ExecuteQueryAsync(Query query, IDictionary<string, object> parameters)
        {
            var freeSqlQuery = query as FreeSqlQuery;
            var sqlQueryResults = new FreeSqlQueryResults();

            if (freeSqlQuery == null)
            {
                return null;
            }

            var tokenizedQuery = await _liquidTemplateManager.RenderStringAsync(freeSqlQuery.Template,
                NullEncoder.Default,
                parameters.Select(x =>
                    new KeyValuePair<string, FluidValue>(x.Key, FluidValue.Create(x.Value, _templateOptions))));

            var dialect = _session.Store.Configuration.SqlDialect;
            try
            {
                var scripts = $"  var params=parameters;\r\n{tokenizedQuery}";
                var builder = await _dynamicIndexAppService.GetIndexAssemblyBuilder();
                //编译查询
                var funcDelegate = FastMethodOperator.UseCompiler(builder)
                    .Param<IDictionary<string, object>>(nameof(parameters))
                    .Body(scripts)
                    .Compile<Func<IDictionary<string, object>, FreeSqlQueryResults>>();
                sqlQueryResults = funcDelegate.Invoke(parameters);
            }
            catch (Exception e)
            {
                sqlQueryResults.Items = Array.Empty<object>();
                sqlQueryResults.TotalCount = 0;
                return sqlQueryResults;
            }

            if (!freeSqlQuery.ReturnDocuments)
            {
                return sqlQueryResults;
            }

            IEnumerable<int> documentIds = sqlQueryResults.Items.Select(Convert.ToInt32);

            sqlQueryResults.Items = await _session.GetAsync<ContentItem>(documentIds.ToArray());
            return sqlQueryResults;
        }
    }
}
