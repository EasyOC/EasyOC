using GraphQL.Types;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using System.Collections.Generic;
using System.Linq;

namespace EaysOC.GraphQL.Queries.Types
{
    public class ContentPickerFieldQueryObjectType : ObjectGraphType<ContentPickerField>
    {
        public ContentPickerFieldQueryObjectType()
        {
            Name = nameof(ContentPickerField);
            Field<StringGraphType>()
                .Name("firstValue")
                .Description("The first content item id in the content picker field.")
                .Resolve(x =>
                {
                    if (x.Source.ContentItemIds != null)
                    {
                        return x.Source?.ContentItemIds.FirstOrDefault();
                    }
                    return null;
                });

            Field<ListGraphType<StringGraphType>, IEnumerable<string>>()
                .Name("contentItemIds")
                .Description("content item ids")
                .PagingArguments()
                .Resolve(x =>
                {
                    if (x.Source.ContentItemIds == null)
                    {
                        return x.Page(new List<string>());
                    }
                    else
                    {
                        return x.Page(x.Source?.ContentItemIds);
                    }
                });

            Field<ContentItemInterface, ContentItem>()
                .Name("firstContentItem")
                .Description("The first content item in the content picker field.")
                .ResolveAsync(async x =>
                {
                    var contentItemLoader = x.GetOrAddPublishedContentItemByIdDataLoader();
                    if (x.Source.ContentItemIds != null && x.Source.ContentItemIds.Any())
                    {
                        var firstValue = x.Source.ContentItemIds.FirstOrDefault(x => x != null);
                        if (firstValue != null)
                        {
                            var result = await contentItemLoader.LoadAsync(firstValue);
                            return result;
                        }

                    }

                    return null;
                });

            Field<ListGraphType<ContentItemInterface>, ContentItem[]>()
                .Name("contentItems")
                .Description("the content items")
                .PagingArguments()
                .ResolveAsync(async x =>
                {
                    var contentItemLoader = x.GetOrAddPublishedContentItemByIdDataLoader();
                    var items = await contentItemLoader.LoadAsync(x.Page(x.Source.ContentItemIds));
                    return items.Where(item => item != null).ToArray();
                });
        }
    }
}
