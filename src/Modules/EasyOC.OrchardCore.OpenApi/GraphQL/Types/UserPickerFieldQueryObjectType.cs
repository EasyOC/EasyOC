using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.Entities;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Users;
using OrchardCore.Users.Models;

namespace EasyOC.OrchardCore.OpenApi.GraphQL.Types
{
    public class UserPickerFieldQueryObjectType : ObjectGraphType<UserPickerField>
    {
        public UserPickerFieldQueryObjectType()
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
                   return x.Source.GetUserNames();
               });

            Field<ListGraphType<ContentItemInterface>, ContentItem[]>()
             .Name("UserProfiles")
             .Description("the user profiles")
             .PagingArguments()
               .ResolveAsync(async x =>
             {
                 var userIds = x.Source.UserIds;
                 var contentItemLoader = x.GetOrAddPublishedContentItemByIdDataLoader();
                 var items = await contentItemLoader.LoadAsync(x.Page(x.Source.UserIds));
                 return items.Where(item => item != null).ToArray();  
             });
        }
    }
}
