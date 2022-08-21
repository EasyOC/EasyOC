using EasyOC.Infrastructure.Security;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using OrchardCore.Security;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class OrchardCoreBuilderExtensions
    {
        /// <summary>
        /// Adds tenant level services.
        /// </summary>
        public static OrchardCoreBuilder AddEocSecurity(this OrchardCoreBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddAuthorization();

                services.Configure<AuthenticationOptions>((options) =>
                {
                    if (options.Schemes.All(x => x.Name != "Api"))
                    {
                        options.AddScheme<EOCApiAuthenticationHandler>("Api", null);
                    }
                });

                services.AddScoped<IPermissionGrantingService, EocPermissionGrantingService>();
                services.AddScoped<IAuthorizationHandler, EocPermissionHandler>();
            });

            return builder;
        }
    }
}
