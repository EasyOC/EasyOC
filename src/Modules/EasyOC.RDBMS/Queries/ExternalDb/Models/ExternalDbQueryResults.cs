using OrchardCore.Queries;
using System.Collections.Generic;

namespace EasyOC.RDBMS.Queries.ExternalDb.Models
{
    public class ExternalDbQueryResults : IQueryResults
    {
        public IEnumerable<object> Items { get; set; }
        public long? Total { get; set; }
    }
}
