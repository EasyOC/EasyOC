using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;

namespace StatCan.OrchardCore.CommonTypes.SecurePage
{
    [Feature(FeatureIds.SecurePage)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection serviceCollection) => serviceCollection.AddScoped<IDataMigration, Migrations>();
    }
}



