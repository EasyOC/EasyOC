using OrchardCore.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.Queries.ScriptQuery;

public class ScriptQuerySource : IQuerySource
{
    public string Name => Consts.QueryName;
    private readonly IScriptQueryService _scriptQueryService;
    public ScriptQuerySource(IScriptQueryService scriptQueryService)
    {
        _scriptQueryService = scriptQueryService;
    }

    public Query Create()
    {
        return new ScriptQuery();
    }

    public async Task<IQueryResults> ExecuteQueryAsync(Query query, IDictionary<string, object> parameters)
    {
        return await _scriptQueryService.ExcuteScriptQuery(query as ScriptQuery, parameters );
    }
   
}