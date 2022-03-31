using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Mutations;
using OrchardCore.ContentManagement.GraphQL.Mutations.Types;

namespace OrchardCore.ContentManagement.GraphQL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContentMutationGraphQL(this IServiceCollection services)
        {
            services.AddSingleton<ISchemaBuilder, CreateContentItemMutationBuilder>();
            services.AddTransient<CreateContentItemInputType>();

            services.AddGraphMutationType<CreateContentItemMutation>();
            services.AddGraphMutationType<DeleteContentItemMutation>();

            services.AddTransient<DeletionStatusObjectGraphType>();
            services.AddTransient<CreateContentItemInputType>();
            services.AddSingleton<ISchemaBuilder, CreateContentItemMutationBuilder>();
            return services;
        }
    }
}
