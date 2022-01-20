using Dapper;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.Options;
using OrchardCore.ContentManagement;
using OrchardCore.Data;
using OrchardCore.Liquid;
using OrchardCore.Queries;
using OrchardCore.Queries.Sql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.RDBMS.Queries.Sql
{
    public class FreeSqlQuerySource : IQuerySource
    {
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly IDbConnectionAccessor _dbConnectionAccessor;
        private readonly ISession _session;
        private readonly TemplateOptions _templateOptions;
        private readonly IFreeSql _freeSql;

        public FreeSqlQuerySource(
            ILiquidTemplateManager liquidTemplateManager,
            IDbConnectionAccessor dbConnectionAccessor,
            ISession session,
            IOptions<TemplateOptions> templateOptions, IFreeSql freeSql)
        {
            _liquidTemplateManager = liquidTemplateManager;
            _dbConnectionAccessor = dbConnectionAccessor;
            _session = session;
            _templateOptions = templateOptions.Value;
            _freeSql = freeSql;
        }

        public string Name => "FreeSql";

        public Query Create()
        {
            return new SqlQuery();
        }

        public async Task<IQueryResults> ExecuteQueryAsync(Query query, IDictionary<string, object> parameters)
        {
            var sqlQuery = query as SqlQuery;
            var sqlQueryResults = new SQLQueryResults();

            var tokenizedQuery = await _liquidTemplateManager.RenderStringAsync(sqlQuery.Template, NullEncoder.Default,
                parameters.Select(x => new KeyValuePair<string, FluidValue>(x.Key, FluidValue.Create(x.Value, _templateOptions))));

            var connection = _dbConnectionAccessor.CreateConnection();
            var dialect = _session.Store.Configuration.SqlDialect;

            if (!SqlParser.TryParse(tokenizedQuery, dialect, _session.Store.Configuration.TablePrefix, parameters, out var rawQuery, out var messages))
            {
                sqlQueryResults.Items = new object[0];
                connection.Dispose();
                return sqlQueryResults;
            }

            if (sqlQuery.ReturnDocuments)
            {
                IEnumerable<int> documentIds;

                using (connection)
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction(_session.Store.Configuration.IsolationLevel))
                    {
                        documentIds = await connection.QueryAsync<int>(rawQuery, parameters, transaction);
                    }
                }

                sqlQueryResults.Items = await _session.GetAsync<ContentItem>(documentIds.ToArray());
                return sqlQueryResults;
            }
            else
            {
                IEnumerable<object> queryResults = new List<object>();
                using (connection)
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction(_session.Store.Configuration.IsolationLevel))
                    {
                        queryResults = await connection.QueryAsync(rawQuery, parameters, transaction);
                    }
                }

                //var results = new List<JObject>();

                //foreach (var document in queryResults)
                //{
                //    results.Add(JObject.FromObject(document));
                //}

                sqlQueryResults.Items = queryResults.ToArray();
                return sqlQueryResults;
            }
        }
    }
}



