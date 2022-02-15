using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Models;
using OrchardCore.Wechat.Services;
using OrchardCore.Wechat.Settings;

namespace OrchardCore.Wechat.Configuration
{
    public class WechatOptionsConfiguration :
        IConfigureOptions<AuthenticationOptions>,
        IConfigureNamedOptions<WechatOptions>
    {
        private readonly IWechatAuthenticationService _wechatAuthenticationService;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ShellSettings _shellSettings;
        private readonly ILogger _logger;

        public WechatOptionsConfiguration(
            IWechatAuthenticationService wechatAuthenticationService,
            IDataProtectionProvider dataProtectionProvider,
            ShellSettings shellSettings,
            ILogger<WechatOptionsConfiguration> logger)
        {
            _wechatAuthenticationService = wechatAuthenticationService;
            _dataProtectionProvider = dataProtectionProvider;
            _shellSettings = shellSettings;
            _logger = logger;
        }

        public void Configure(AuthenticationOptions options)
        {
            var settings = WechatAuthenticationSettingsAsync().GetAwaiter().GetResult();
            if (settings == null)
            {
                return;
            }

            if (_wechatAuthenticationService.ValidateSettings(settings).Any())
            {
                return;
            }

            // Register the OpenID Connect client handler in the authentication handlers collection.
            options.AddScheme(WechatDefaults.AuthenticationScheme, builder =>
            {
                builder.DisplayName = "Wechat";
                builder.HandlerType = typeof(WechatHandler);
            });
        }

        public void Configure(string name, WechatOptions options)
        {
            // Ignore OpenID Connect client handler instances that don't correspond to the instance managed by the OpenID module.
            if (!String.Equals(name, WechatDefaults.AuthenticationScheme))
            {
                return;
            }

            var loginSettings = WechatAuthenticationSettingsAsync().GetAwaiter().GetResult();
            if (loginSettings == null)
            {
                return;
            }

            options.ClientId = loginSettings.ClientID;
            options.AppId = loginSettings.AppId;
           

            try
            {
                options.ClientSecret = _dataProtectionProvider.CreateProtector(WechatConstants.Features.WechatAuthentication).Unprotect(loginSettings.ClientSecret);
                options.AppSecret = _dataProtectionProvider.CreateProtector(WechatConstants.Features.WechatAuthentication).Unprotect(loginSettings.AppSecret);
            }
            catch
            {
                _logger.LogError("The Wechat Consumer Secret could not be decrypted. It may have been encrypted using a different key.");
            }

            if (loginSettings.CallbackPath.HasValue)
            {
                options.CallbackPath = loginSettings.CallbackPath;
            }

            options.SaveTokens = loginSettings.SaveTokens;
            options.EnableOfficialAccount = loginSettings.EnableOfficialAccount;
        }

        public void Configure(WechatOptions options) => Debug.Fail("This infrastructure method shouldn't be called.");

        private async Task<WechatAuthenticationSettings> WechatAuthenticationSettingsAsync()
        {
            var settings = await _wechatAuthenticationService.GetSettingsAsync();
            if ((_wechatAuthenticationService.ValidateSettings(settings)).Any(result => result != ValidationResult.Success))
            {
                if (_shellSettings.State == TenantState.Running)
                {
                    _logger.LogWarning("Wechat Authentication is not correctly configured.");
                }

                return null;
            }

            return settings;
        }
    }
}
