using EasyOC.RDBMS.Queries.ScriptQuery;
using OrchardCore.Queries;

namespace EasyOC.Scripting.Queries.ScriptQuery
{
    public class ScriptQuery : Query
    {
        public ScriptQuery() : base(Consts.QueryName)
        {
        }

        public string Scripts { get; set; }
        public bool ReturnDocuments { get; set; }
    }
    public class ScriptQueryTesting : ScriptQuery
    {
    };


}