using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrchardCore.ContentManagement.GraphQL;
using GraphQL.DataLoader;

namespace EasyOC.GraphQL.Extensions
{
    public static class GraphqlExtensions
    {
        public static IServiceProvider ContentPickerFieldQueryObjectTypePatch(this IServiceProvider serviceProvider)
        {

            var pickerType = serviceProvider.GetRequiredService<ContentPickerFieldQueryObjectType>();
            pickerType.Field<StringGraphType>()
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
            pickerType.Field<ContentItemInterface, ContentItem>()
                 .Name("firstContentItem")
                 .Description("The first content item in the content picker field.")
                 .ResolveAsync(x =>
                 {
                     var contentItemLoader = x.GetOrAddPublishedContentItemByIdDataLoader();
                     if (x.Source.ContentItemIds != null && x.Source.ContentItemIds.Any())
                     {
                         var firstValue = x.Source.ContentItemIds.FirstOrDefault(x => x != null);
                         if (firstValue != null)
                         {
                             var result = contentItemLoader.LoadAsync(firstValue);
                             return result.Then(x => x.FirstOrDefault());
                         }
                     }
                     return null;
                 });

            return serviceProvider;
        }
    }
}
