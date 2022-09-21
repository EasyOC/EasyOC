using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.Queries.ExternalDb.Models
{
    public class ExternalDbQueryResults : IQueryResults
    {
        public IEnumerable<object> Items { get; set; }
        public long? Total { get; set; }
    }
}
