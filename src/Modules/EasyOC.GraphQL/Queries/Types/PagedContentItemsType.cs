using GraphQL.Types;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC.GraphQL.Queries.Types
{
    public class PagedContentItemsType : ObjectGraphType<PagedContentItemResult>
    {
        public PagedContentItemsType()
        {
            Name = "pagedContentItemsQuery";
            Field<ListGraphType<ContentItemInterface>, IEnumerable<ContentItem>>()
                .Name("items")
                .Description("the content items")
                .Resolve(x =>
                {
                    return x.Source?.Items;
                });
            Field<IntGraphType>("total", resolve: context => context.Source.Total);
            Description = "A paged collection of content items";

        }
    }
}
