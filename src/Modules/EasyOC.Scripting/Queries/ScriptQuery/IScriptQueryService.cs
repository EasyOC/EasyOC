using EasyOC.Scripting.Queries.ScriptQuery.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.Scripting.Queries.ScriptQuery
{
    public interface IScriptQueryService
    {
        Task<ScriptQueryResult> ExcuteScriptQuery(ScriptQuery extDbQuery, IDictionary<string, object> parameters );
    }
}