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

namespace EasyOC.OpenApi.GraphQL.Types
{
    public class UserPickerFieldQueryObjectType : ObjectGraphType<UserPickerField>
    {
        public UserPickerFieldQueryObjectType()
        {
            Name = nameof(UserPickerField);
            Field<StringGraphType>()
                .Name("firstValue")
                .Description("The first userId in the user picker field.")
                .Resolve(x =>
                {
                    if (x.Source.UserIds != null)
                    {
                        return x.Source?.UserIds.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                });

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
            Field<ContentItemInterface, ContentItem>()
                .Name("firstUserProfiles")
                .Description("The first UserProfiles in the user picker field.")
                .ResolveAsync(async x =>
                {
                    if (x.Source.UserIds == null || !x.Source.UserIds.Any())
                    {
                        return null;
                    }

                    var contentItemLoader = x.GetOrAddPublishedContentItemByIdDataLoader();
                    var profile = await contentItemLoader.LoadAsync(x.Source.UserIds.FirstOrDefault());
                    return profile;
                });
            Field<ListGraphType<ContentItemInterface>, ContentItem[]>()
                .Name("UserProfiles")
                .Description("the user profiles")
                .PagingArguments()
                .ResolveAsync(async x =>
                {
                    if (x.Source.UserIds == null || !x.Source.UserIds.Any())
                    {
                        return null;
                    }
                    var userIds = x.Source.UserIds;
                    var contentItemLoader = x.GetOrAddPublishedContentItemByIdDataLoader();
                    var items = await contentItemLoader.LoadAsync(x.Page(userIds));
                    return items.Where(item => item != null).ToArray();
                });
        }
    }
}
