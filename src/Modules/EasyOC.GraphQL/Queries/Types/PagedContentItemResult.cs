using OrchardCore.ContentManagement;
using System.Collections.Generic;

namespace EasyOC.GraphQL.Queries.Types
{
    public class PagedContentItemResult
    {
        public IEnumerable<ContentItem> Items { get; set; }

        public int? Total { get; set; }
    }
}
