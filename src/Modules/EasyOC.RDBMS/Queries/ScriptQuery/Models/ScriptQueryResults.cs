using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.Queries.ScriptQuery.Models
{
    public class ScriptQueryResults : IQueryResults
    {
        public IEnumerable<object> Items { get; set; }
        public long? Total { get; set; }
        public object? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }

}
