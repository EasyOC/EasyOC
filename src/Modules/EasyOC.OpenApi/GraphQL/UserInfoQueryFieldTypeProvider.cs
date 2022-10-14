using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OrchardCore.Apis.GraphQL;
using OrchardCore.Apis.GraphQL.Resolvers;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.Entities;
using OrchardCore.Queries;
using OrchardCore.Users;
using OrchardCore.Users.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.OpenApi.GraphQL
{
    public class UserInfoQueryFieldTypeProvider : ISchemaBuilder
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer S;

        public UserInfoQueryFieldTypeProvider(IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<UserInfoQueryFieldTypeProvider> localizer)
        {
            _httpContextAccessor = httpContextAccessor;

            S = localizer;
        }

        public Task BuildAsync(ISchema schema)
        {
            var typetype = schema.Query.Fields.OfType<ContentItemsFieldType>().FirstOrDefault(x => x.Name == "UserProfile");
            if (typetype == null)
            {
                return null;
            }
            var field = new FieldType
            {
                Name = "Me",
                Description = S["Information about the current login user."],
                Type = typeof(ContentItemInterface),
                //ResolvedType= typetype.ResolvedType,
                Resolver = new AsyncFieldResolver<ContentItem>(ResolveAsync)
            };

            schema.Query.AddField(field);

            return Task.CompletedTask;
        }

        private async Task<ContentItem> ResolveAsync(IResolveFieldContext context)
        {
            var serviceProvider = _httpContextAccessor.HttpContext.RequestServices;
            var userManager = serviceProvider.GetRequiredService<UserManager<IUser>>();
            var user = await userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name) as User;
            var contentManager = serviceProvider.GetService<IContentManager>();
            return await contentManager.GetAsync(user.UserId);
        }

        public Task<string> GetIdentifierAsync()
        {
            var queryManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IQueryManager>();
            return queryManager.GetIdentifierAsync();
        }
    }
}
