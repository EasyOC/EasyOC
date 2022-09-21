using EasyOC.RDBMS.Queries.ScriptQuery.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.Queries.ScriptQuery
{
    public interface IScriptQueryService
    {
        Task<ScriptQueryResults> ExcuteScriptQuery(ScriptQuery extDbQuery, IDictionary<string, object> parameters );
    }
}