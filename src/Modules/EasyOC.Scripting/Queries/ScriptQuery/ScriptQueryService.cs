using EasyOC.RDBMS;
using EasyOC.RDBMS.Models;
using EasyOC.RDBMS.Queries.ScriptQuery;
using EasyOC.RDBMS.Services;
using EasyOC.Scripting.Queries.ScriptQuery.Models;
using EasyOC.Scripting.Servicies;
using Jint;
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
        private readonly IContentManager _contentManager;
        private readonly IDbAccessableJSScopeBuilder _dbAccessableJSScopeBuilder;

        private readonly ILogger _logger;
        public ScriptQueryService(
            ILogger<ScriptQuerySource> logger,
            IContentManager contentManager,
            IDbAccessableJSScopeBuilder dbAccessableJSScopeBuilder)
        {
            _logger = logger;
            _contentManager = contentManager;
            _dbAccessableJSScopeBuilder = dbAccessableJSScopeBuilder;
        }



        public async Task<ScriptQueryResult> ExcuteScriptQuery(ScriptQuery extDbQuery, IDictionary<string, object> parameters)
        {
            ScriptQueryResult scriptResults = new ScriptQueryResult();
            try
            {
                var scriptScope = await _dbAccessableJSScopeBuilder.CreateScopeAsync();
                var engine = scriptScope.Engine;
                //注入页面参数为js变量
                engine.SetValue("parameters", parameters);
                var paserOptions = new Esprima.ParserOptions();
                var jsValue = engine.Evaluate(extDbQuery.Scripts, paserOptions);
                var value = jsValue.ToObject();
                if (value == null)
                {
                    return null;
                }
                var jToken = JToken.FromObject(value);
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
                    if (jToken is JArray jArray)
                    {
                        scriptResults.Items = jArray.AsEnumerable();
                        scriptResults.Total = scriptResults.Items.Count();
                        return scriptResults;
                    }
                    if (jToken is JObject jobj)
                    {
                        scriptResults = jobj.ToObject<ScriptQueryResult>();
                        if (scriptResults.Data == null)
                        {
                            scriptResults.Data = jobj;
                        }
                        return scriptResults;
                    }

                    if (value is object[] list)
                    {
                        scriptResults.Items = list;
                        scriptResults.Total = list.Length;
                        return scriptResults;
                    }

                    //Holding default
                    scriptResults.Data = JToken.FromObject(value);
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