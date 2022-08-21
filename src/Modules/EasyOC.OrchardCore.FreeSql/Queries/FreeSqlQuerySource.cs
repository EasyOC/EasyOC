using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyOC.OrchardCore.DynamicTypeIndex.Service;
using Fluid;
using Microsoft.Extensions.Options;
using Natasha.CSharp;
using OrchardCore.ContentManagement;
using OrchardCore.Queries;
using System;
using YesSql;

namespace EasyOC.OrchardCore.FreeSql.Queries
{
    public class FreeSqlQuerySource : IQuerySource
    {
        private readonly ISession _session;
        private readonly IDynamicIndexAppService _dynamicIndexAppService;
        private readonly IFreeSql _freeSql;

        public FreeSqlQuerySource(
            ISession session,
            IOptions<TemplateOptions> templateOptions, IDynamicIndexAppService dynamicIndexAppService, IFreeSql freeSql)
        {
            _session = session;
            _dynamicIndexAppService = dynamicIndexAppService;
            _freeSql = freeSql;
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
            // 使用Liquid 模板生成FreeSql查询
            // var tokenizedQuery = await _liquidTemplateManager.RenderStringAsync(freeSqlQuery.Template,
            //     NullEncoder.Default,
            //     parameters.Select(x =>
            //         new KeyValuePair<string, FluidValue>(x.Key, FluidValue.Create(x.Value, _templateOptions))));

            try
            {
                var scripts = $"  var params=parameters;\r\n{freeSqlQuery.Template}";
                var builder = await _dynamicIndexAppService.GetIndexAssemblyBuilder();

                //编译查询
                var funcDelegate = FastMethodOperator.UseCompiler(builder)
                    .Using("OrchardCore.ContentManagement.Records")
                    .Param<IDictionary<string, object>>(nameof(parameters))
                    .Body(scripts)
                    .Compile<Func<IFreeSql, IDictionary<string, object>, FreeSqlQueryResults>>();

                sqlQueryResults = funcDelegate.Invoke(_freeSql, parameters);
                //Sample Codes
                // var fquery = _freeSql.Select<ContentItemIndex>();
                // var result = new FreeSqlQueryResults { TotalCount = fquery.Count(), Items = fquery.ToList() };
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
