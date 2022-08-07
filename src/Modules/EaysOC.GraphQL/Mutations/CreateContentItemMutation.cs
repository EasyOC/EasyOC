using EasyOC.GraphQL.Abstractions.Mutations;
using EasyOC.GraphQL.Abstractions.Types;
using GraphQL.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.Modules;
using System.Collections.Generic;

namespace OrchardCore.ContentManagement.GraphQL.Mutations
{
    public class CreateContentItemMutation : MutationFieldType
    {
        public CreateContentItemMutation(
            IHttpContextAccessor httpContextAccessor,
            IClock clock)
        {
            Name = "CreateContentItem";

            Type = typeof(ContentItemInterface);

            Resolver = new AsyncFieldResolver<object, object>(async (context) =>
            {
                var contentItemFabrication = context.GetArgument<ContentItemInput>("contentItem");

                var contentParts = JObject.FromObject(contentItemFabrication.ContentParts);

                var contentManager = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IContentManager>();
                var contentItem = await contentManager.NewAsync(contentItemFabrication.ContentType);

                contentItem.Author = contentItemFabrication.Author;
                contentItem.Owner = contentItemFabrication.Owner;
                contentItem.CreatedUtc = clock.UtcNow;

                var apiUpdateModel = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IApiUpdateModel>();
                var updateModel = apiUpdateModel.WithModel(contentParts);

                var contentDisplay = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IContentItemDisplayManager>();
                await contentDisplay.UpdateEditorAsync(contentItem, updateModel, true);

                if (contentItemFabrication.Published)
                {
                    // TODO : Auth check for publish
                    await contentManager.CreateAsync(contentItem, VersionOptions.Published);
                }
                else
                {
                    await contentManager.CreateAsync(contentItem, VersionOptions.Latest);
                }

                return contentItem;
            });
        }

        private class ContentItemInput
        {
            public string ContentType { get; set; }
            public string Author { get; set; }
            public string Owner { get; set; }
            public bool Published { get; set; }
            public bool Latest { get; set; }

            public IDictionary<string, object> ContentParts { get; set; } = new Dictionary<string, object>();
        }

        public class ContentPartInput
        {
            public string Name { get; set; }
            public string Content { get; set; }
        }
    }
}
