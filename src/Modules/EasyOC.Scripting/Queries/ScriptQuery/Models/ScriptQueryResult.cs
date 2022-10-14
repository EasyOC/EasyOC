using OrchardCore.Queries;
using System.Collections.Generic;

namespace EasyOC.Scripting.Queries.ScriptQuery.Models
{
    public class ScriptQueryResult : IQueryResults
    {
        public IEnumerable<object> Items { get; set; }
        public long? Total { get; set; }
        public object? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }

}
