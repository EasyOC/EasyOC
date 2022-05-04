using EasyOC.OrchardCore.ContentExtentions.GraphQL.Types;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.Modules;

namespace EasyOC.OrchardCore.ContentExtentions.GraphQL;

[RequireFeatures("OrchardCore.Apis.GraphQL")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ISchemaBuilder, ContentItemByVersionQuery>();
        
        services.AddSingleton<ContentPickerFieldQueryObjectType>();
        services.Replace(ServiceDescriptor.Singleton<ObjectGraphType<ContentPickerField>, ContentPickerFieldQueryObjectType>
                            (s => s.GetRequiredService<ContentPickerFieldQueryObjectType>()));
        services.Replace(ServiceDescriptor.Singleton<IObjectGraphType, ContentPickerFieldQueryObjectType>
                            (s => s.GetRequiredService<ContentPickerFieldQueryObjectType>()));

    }
}



