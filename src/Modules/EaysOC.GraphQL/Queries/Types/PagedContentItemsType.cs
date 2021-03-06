using GraphQL.Types;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using System.Collections.Generic;
using System.Linq;

namespace EaysOC.GraphQL.Queries.Types
{
    public class PagedContentItemsType : ObjectGraphType<TotalQueryResults>
    {
        public PagedContentItemsType()
        {
            Name = "pagedContentItemsQuery";
            Field<ListGraphType<ContentItemInterface>, IEnumerable<ContentItem>>()
                .Name("items")
                .Description("the content items")
                .Resolve(x =>
                {
                    if (x.Source?.Items == null || x.Source?.Items?.Count() == 0)
                    {
                        return Enumerable.Empty<ContentItem>();
                    }
                    return x.Source?.Items?.Select(i => i as ContentItem);
                });
            Field<IntGraphType>("total", resolve: context => context.Source.Total);
            Description = "A paged collection of content items";

        }
    }
}
