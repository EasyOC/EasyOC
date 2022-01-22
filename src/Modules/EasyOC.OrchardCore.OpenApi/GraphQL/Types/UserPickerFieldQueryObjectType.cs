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
             .Name("properties")
             .Description("the user properties")
             .PagingArguments()
               .Resolve(x =>
             {
                 var userIds = x.Source.UserIds;
                 if (userIds != null && userIds.Length > 0)
                 {
                     var serviceProvider = x.ResolveServiceProvider();
                     var userManager = serviceProvider.GetRequiredService<UserManager<IUser>>();

                     var items = userIds.Select(id =>
                   {
                       var u = userManager.FindByIdAsync(id).GetAwaiter().GetResult();
                       if (u != null)
                       {
                           var profile = ((User)u).As<ContentItem>("UserProfile");
                           return profile;
                       }
                       return null;

                   });
                     return items.Where(u => u != null).ToArray();

                 }
                 else
                 {
                     return Array.Empty<ContentItem>();
                 }

             });
        }
    }
}
