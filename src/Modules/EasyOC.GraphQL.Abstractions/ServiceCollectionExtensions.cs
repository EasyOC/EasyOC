using EasyOC.GraphQL.Abstractions.Types;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL.Queries;

namespace EasyOC.GraphQL.Abstractions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGraphMutationType<TMutation>(this IServiceCollection services) where TMutation : MutationFieldType
        {
            services.AddTransient<TMutation>();
            services.AddTransient<MutationFieldType, TMutation>();
        }

        public static void AddGraphQLInputType<TInput, TInputType>(this IServiceCollection services)
            where TInput : class
            where TInputType : InputObjectGraphType<TInput>
        {
            services.AddTransient<InputObjectGraphType<TInput>, TInputType>();
            services.AddTransient<IInputObjectGraphType, TInputType>();
        }

    }
}
