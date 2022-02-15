using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Wechat.Configuration;
using OrchardCore.Wechat.Drivers;
using OrchardCore.Wechat.Recipes;
using OrchardCore.Wechat.Services;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Recipes;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;

namespace OrchardCore.Wechat
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPermissionProvider, Permissions>();
        }
    }

    [Feature(WechatConstants.Features.WechatAuthentication)]
    public class WechatLoginStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IWechatAuthenticationService, WechatAuthenticationService>();
            services.AddScoped<IDisplayDriver<ISite>, WechatAuthenticationSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenuWechatLogin>();
            services.AddRecipeExecutionStep<WechatAuthenticationSettingsStep>();
            // Register the options initializers required by the Wechat Handler.
            services.TryAddEnumerable(new[]
            {
                // Orchard-specific initializers:
                ServiceDescriptor.Transient<IConfigureOptions<AuthenticationOptions>, WechatOptionsConfiguration>(),
                ServiceDescriptor.Transient<IConfigureOptions<WechatOptions>, WechatOptionsConfiguration>(),
                // Built-in initializers:
                ServiceDescriptor.Transient<IPostConfigureOptions<WechatOptions>, OAuthPostConfigureOptions<WechatOptions,WechatHandler>>()
            });
        }
    }
}
