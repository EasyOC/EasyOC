using EasyOC.RDBMS.Models;
using EasyOC.RDBMS.Queries.ScriptQuery.Models;
using EasyOC.RDBMS.Services;
using GraphQL;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Queries;
using OrchardCore.Scripting;
using OrchardCore.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.Queries.ScriptQuery;

public class ScriptQuerySource : IQuerySource
{
    public string Name => Consts.QueryName;
    private readonly IScriptingManager _scriptingManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly IContentManager _contentManager;
    private readonly IRDBMSAppService _rDbmsAppService;
    private readonly ILogger _logger;
    public ScriptQuerySource(IScriptingManager scriptingManager, IServiceProvider serviceProvider,
        ILogger<ScriptQuerySource> logger, IContentManager contentManager, IRDBMSAppService rDbmsAppService)
    {
        _scriptingManager = scriptingManager;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _contentManager = contentManager;
        _rDbmsAppService = rDbmsAppService;
    }

    public Query Create()
    {
        return new ScriptQuery();
    }

    public async Task<IQueryResults> ExecuteQueryAsync(Query query, IDictionary<string, object> parameters)
    {
        var extDbQuery = query as ScriptQuery;
        var scriptResults = new ScriptQueryResults();

        if (extDbQuery == null)
        {
            return null;
        }

        try
        {
            var scope = _scriptingManager.CreateScope("js", _serviceProvider)
                as JavaScriptScope;
            var engine = scope.Engine;

            //注入页面参数为js变量
            engine.SetValue("params", parameters);
            var connections = await _rDbmsAppService.GetAllDbConnection();

            foreach (var item in connections)
            {
                engine.SetValue(item.ConfigName, new ExternalDbProvider(_serviceProvider, new ExternalDbConfig
                {
                    Name = item.ConfigName,
                    ConnectionConfigId = item.ConfigId
                }, _logger));
            }
            engine.SetValue(Constants.ShellDbName, new ExternalDbProvider(_serviceProvider, new ExternalDbConfig
            {
                UseShellDb = true
            }, _logger));
            var value = await Task.Run(() => engine.Evaluate(extDbQuery.Scripts).GetValue());
            if (extDbQuery.ReturnDocuments && value is JArray jArray1)
            {
                var ids = jArray1.Values<string>();
                scriptResults.Items = await _contentManager.GetAsync(ids);
                return scriptResults;
            }
            else
            {
                if (value is JObject jObj)
                {
                    scriptResults = jObj.ToObject<ScriptQueryResults>();
                    return scriptResults;
                }
                JArray jArray;
                if ((jArray = value as JArray) != null)
                {
                    scriptResults.Items = jArray.Children().Aggregate(new List<object>(), delegate(List<object> list, JToken token) {
                        list.Add(token.GetValue());
                        return list;
                    });
                    return scriptResults;
                }
                //Holding default
                scriptResults.Data = value;
                return scriptResults;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("{queryName} 查询执行失败，处理程序：{providerName}--{error}", query.Name, nameof(ScriptQuerySource), e);
            if (scriptResults?.Message is null)
            {
                scriptResults.Message = "操作失败";
            }
            scriptResults.Success = false;
            return scriptResults;
        }
    }
}