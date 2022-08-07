using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.ContentFields.Fields;
using System;

namespace EasyOC;

public static class GraphicalExtensions
{
    public static void ReplaceObjectGraphType<TInput,TInputType>(this IServiceCollection services )
        where TInputType :  ObjectGraphType<TInput>
    {
        services.AddSingleton<TInputType>();
        services.Replace(ServiceDescriptor.Singleton<ObjectGraphType<TInput>, TInputType>
            (s => s.GetRequiredService<TInputType>()));
        services.Replace(ServiceDescriptor.Singleton<IObjectGraphType, TInputType>
            (s => s.GetRequiredService<TInputType>()));


    }


}
