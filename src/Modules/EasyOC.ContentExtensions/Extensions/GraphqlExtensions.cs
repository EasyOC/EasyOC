using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Apis;
using System.Linq;

namespace EasyOC;

public static class GraphqlExtensions
{
    public static void ReplaceObjectGraphType<TTargetInputType,TInput, TInputType>(this IServiceCollection services)
            where TInput : class
            where TTargetInputType : ObjectGraphType<TInput>
            where TInputType : ObjectGraphType<TInput>
    {

        var targetService = services.FirstOrDefault(x => x.ServiceType == typeof(TTargetInputType));
        services.Remove(targetService);
        var objT = services.FirstOrDefault(x => x.ServiceType == typeof(ObjectGraphType<TInput>) 
                            && x.ImplementationFactory.Method.ReturnType == typeof(TTargetInputType));
        services.Remove(objT);
        var objGtype = services.FirstOrDefault(x => x.ServiceType == typeof(IObjectGraphType)
                            && x.ImplementationFactory.Method.ReturnType == typeof(TTargetInputType));
        services.Remove(objGtype);

        services.AddObjectGraphType<TInput, TInputType>(); 
    }
}
