using EasyOC.RDBMS.Queries.ExternalDb.Models;
using EasyOC.RDBMS.Services;
using GraphQL;
using Jint;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.Queries;
using OrchardCore.Scripting;
using OrchardCore.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.Queries.ExternalDb
{
    public class ExternalDbQuerySource : IQuerySource
    {
        public string Name => Consts.QueryName;
        private readonly IScriptingManager _scriptingManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRDBMSAppService _rDBMSAppService;
        private readonly ILogger _logger;
        public ExternalDbQuerySource(IScriptingManager scriptingManager, IServiceProvider serviceProvider,
            IRDBMSAppService rDBMSAppService, ILogger<ExternalDbQuerySource> logger)
        {
            _scriptingManager = scriptingManager;
            _serviceProvider = serviceProvider;
            _rDBMSAppService = rDBMSAppService;
            _logger = logger;
        }

        public Query Create()
        {
            return new ExternalDbQuery();
        }

        public async Task<IQueryResults> ExecuteQueryAsync(Query query, IDictionary<string, object> parameters)
        {
            var extDbQuery = query as ExternalDbQuery;
            var sqlQueryResults = new ExternalDbQueryResults();

            if (extDbQuery == null)
            {
                return null;
            }

            try
            {
                var scope = _scriptingManager.CreateScope("js", _serviceProvider) as JavaScriptScope;
                var engine = scope.Engine;
                //注入页面参数为js变量
                engine.SetValue("params", parameters);
                var sqlText = engine.Evaluate(extDbQuery.Source).GetValue().ToString();
                //获取连接对象
                var fsql = await _rDBMSAppService.GetFreeSqlAsync(extDbQuery.ConnectionConfigId);
                sqlQueryResults.Items = await fsql.Ado.QueryAsync<object>(sqlText, parameters);
                //启用行数统计
                if (extDbQuery.HasTotal)
                {
                    //行数统计脚本
                    var totalText = engine
                        .Evaluate(extDbQuery.TotalQuery)
                        .GetValue().ToString();
                    sqlQueryResults.Total = await fsql.Ado.QuerySingleAsync<long>(sqlText, parameters);
                }
                return sqlQueryResults;
            }
            catch (Exception e)
            {
                _logger.LogError($"{query.Name} 查询执行失败，处理程序：{nameof(ExternalDbQuerySource)},", e);
                sqlQueryResults.Items = Array.Empty<object>();
                sqlQueryResults.Total = 0;
                return sqlQueryResults;
            }
        }
    }
}
