using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Modules;
using StatCan.OrchardCore.ContentFields.PredefinedGroup.Settings;

namespace StatCan.OrchardCore.ContentFields.PredefinedGroup
{
    [Feature(Constants.Features.PredefinedGroup)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IContentPartFieldDefinitionDisplayDriver, TextFieldPredefinedGroupEditorSettingsDriver>();
        }
    }
}


