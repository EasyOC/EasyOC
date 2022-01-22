using System.Collections.Generic;
using GraphQL.Types;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.GraphQL;

namespace EasyOC.OrchardCore.OpenApi.GraphQL.Types
{
    public class ContentPickerFieldQueryObjectType : ObjectGraphType<UserPickerField>
    {
        public ContentPickerFieldQueryObjectType()
        {
            Name = nameof(ContentPickerField);

            Field<ListGraphType<StringGraphType>, IEnumerable<string>>()
                .Name("userIds")
                .Description("content item ids")
                .PagingArguments()
                .Resolve(x =>
                {
                    return x.Source.UserIds;
                });

            Field<ListGraphType<StringGraphType>, IEnumerable<string>>()
                .Name("userNames")
                .Description("the userName")
                .PagingArguments()
                .Resolve(x =>
               {
                   var contentItemLoader = x.GetOrAddPublishedContentItemByIdDataLoader();
                   return x.Source.GetUserNames();
               });
        }
    }
}
