using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.Modules;

namespace Lombiq.HelpfulExtensions.Extensions.ShapeTracing
{
    [Feature(FeatureIds.ShapeTracing)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services) =>
            services.AddScoped<IShapeDisplayEvents, ShapeTracingShapeEvents>();
    }
}



