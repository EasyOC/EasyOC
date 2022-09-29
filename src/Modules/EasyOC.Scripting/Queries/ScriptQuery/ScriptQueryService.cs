using EasyOC.RDBMS;
using EasyOC.RDBMS.Models;
using EasyOC.RDBMS.Queries.ScriptQuery;
using EasyOC.RDBMS.Services;
using EasyOC.Scripting.Queries.ScriptQuery.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Scripting;
using OrchardCore.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.Scripting.Queries.ScriptQuery
{
    public class ScriptQueryService : IScriptQueryService
    {
        private readonly IScriptingManager _scriptingManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IContentManager _contentManager;
        private readonly IRDBMSAppService _rDbmsAppService;
        private readonly ILogger _logger;
        public ScriptQueryService(IScriptingManager scriptingManager, IServiceProvider serviceProvider,
            ILogger<ScriptQuerySource> logger, IContentManager contentManager, IRDBMSAppService rDbmsAppService)
        {
            _scriptingManager = scriptingManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _contentManager = contentManager;
            _rDbmsAppService = rDbmsAppService;
        }
        public async Task<ScriptQueryResult> ExcuteScriptQuery(ScriptQuery extDbQuery, IDictionary<string, object> parameters)
        {
            ScriptQueryResult scriptResults = null;
            try
            {
                var scope = _scriptingManager.CreateScope("js", _serviceProvider)
                    as JavaScriptScope;
                var engine = scope.Engine;

                //注入页面参数为js变量
                engine.SetValue("parameters", parameters);
                #region 注册数据库访问对象
                var connections = await _rDbmsAppService.GetAllDbConnection();

                foreach (var item in connections)
                {
                    engine.SetValue(item.ConfigName, new ExternalDbProvider(_serviceProvider, new ExternalDbConfig
                    {
                        Name = item.ConfigName,
                        ConnectionConfigId = item.ConfigId
                    }, _logger));
                }
                engine.SetValue(RDBMS.Constants.ShellDbName, new ExternalDbProvider(_serviceProvider, new ExternalDbConfig
                {
                    Name = RDBMS.Constants.ShellDbName,
                    ConnectionConfigId = RDBMS.Constants.ShellDbName
                }, _logger));
                #endregion

                var jsValue = engine.Evaluate(extDbQuery.Scripts);
                var value = jsValue.ToObject();
                if (extDbQuery.ReturnDocuments)
                {
                    if (jsValue.IsArray())
                    {
                        var ids = Array.ConvertAll(value as object[], x => x.ToString());
                        scriptResults.Items = await _contentManager.GetAsync(ids);
                        scriptResults.Total = scriptResults.Items.Count();
                        return scriptResults;
                    }
                    scriptResults.Message = "需要返回文档请直接返回 DocumentId 字符串集合";
                    throw new Exception(scriptResults.Message);
                }
                else
                {
                    var jobj = JObject.FromObject(value);
                    scriptResults = jobj.ToObject<ScriptQueryResult>();
                    if (scriptResults != null)
                    {
                        return scriptResults;
                    }
                    if (value is object[] list)
                    {
                        scriptResults.Items = list;
                        scriptResults.Total = list.Length;
                        return scriptResults;
                    }
                    //Holding default
                    scriptResults.Data = value;
                    return scriptResults;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("{queryName} 查询执行失败，处理程序：{providerName}--{error}", extDbQuery.Name, nameof(ScriptQuerySource), e);
                if (scriptResults?.Message is null)
                {
                    scriptResults.Message = "操作失败";
                }
                if (extDbQuery is ScriptQueryTesting)
                {
                    scriptResults.Message = $"{extDbQuery.Name} 查询执行失败，处理程序：{nameof(ScriptQuerySource)}--{e}";
                }
                scriptResults.Success = false;
                return scriptResults;
            }
        }
    }
}