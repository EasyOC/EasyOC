using OrchardCore.Queries;
using System.Collections.Generic;

namespace EasyOC.FreeSql.Queries;

public class FreeSqlQueryResults : IQueryResults
{
    public IEnumerable<object> Items { get; set; }
    public long TotalCount { get; set; }
}
