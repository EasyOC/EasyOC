using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL;
using OrchardCore.Modules;

namespace EasyOC.OrchardCore.ContentExtentions.GraphQL;

[RequireFeatures("OrchardCore.Apis.GraphQL")]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    { 
        services.AddSingleton<ISchemaBuilder, ContentItemByVersionQuery>();
    } 
}



