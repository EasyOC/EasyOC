using EasyOC.OrchardCore.ContentExtentions.GraphQL.Types;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.GraphQL.Queries;
using OrchardCore.Modules;
using System.Linq;

namespace EasyOC.OrchardCore.ContentExtentions.GraphQL;

[RequireFeatures("OrchardCore.Apis.GraphQL")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ISchemaBuilder, ContentItemByVersionQuery>();

        // services.Remove(ServiceDescriptor.Singleton<ISchemaBuilder, ContentTypeQuery>());
        services.AddSingleton<ISchemaBuilder, EOCContentTypeQuery>();

        services.ReplaceObjectGraphType<ContentPickerField, ContentPickerFieldQueryObjectType>();
        // services.AddSingleton<ContentPickerFieldQueryObjectType>();
        // services.Replace(ServiceDescriptor.Singleton<ObjectGraphType<ContentPickerField>, ContentPickerFieldQueryObjectType>
        //                     (s => s.GetRequiredService<ContentPickerFieldQueryObjectType>()));
        // services.Replace(ServiceDescriptor.Singleton<IObjectGraphType, ContentPickerFieldQueryObjectType>
        //                     (s => s.GetRequiredService<ContentPickerFieldQueryObjectType>()));
    }
}
