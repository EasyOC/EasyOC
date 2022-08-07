using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using OrchardCore.Wechat.Settings;
using OrchardCore.Wechat.ViewModels;
using OrchardCore.Settings;

namespace OrchardCore.Wechat.Drivers
{
    public class WechatAuthenticationSettingsDisplayDriver : SectionDisplayDriver<ISite, WechatAuthenticationSettings>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IShellHost _shellHost;
        private readonly ShellSettings _shellSettings;
        private readonly ILogger _logger;

        public WechatAuthenticationSettingsDisplayDriver(
            IAuthorizationService authorizationService,
            IDataProtectionProvider dataProtectionProvider,
            IHttpContextAccessor httpContextAccessor,
            IShellHost shellHost,
            ShellSettings shellSettings,
            ILogger<WechatAuthenticationSettingsDisplayDriver> logger)
        {
            _authorizationService = authorizationService;
            _dataProtectionProvider = dataProtectionProvider;
            _httpContextAccessor = httpContextAccessor;
            _shellHost = shellHost;
            _shellSettings = shellSettings;
            _logger = logger;
        }

        public override async Task<IDisplayResult> EditAsync(WechatAuthenticationSettings settings, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (!await _authorizationService.AuthorizeAsync(user, Permissions.ManageWechatAuthentication))
            {
                return null;
            }

            return Initialize<WechatAuthenticationSettingsViewModel>("WechatAuthenticationSettings_Edit", model =>
            {
                model.ClientID = settings.ClientID;
                model.AppId = settings.AppId;
                
                if (!string.IsNullOrWhiteSpace(settings.ClientSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(WechatConstants.Features.WechatAuthentication);
                        model.ClientSecret = protector.Unprotect(settings.ClientSecret);
                    }
                    catch (CryptographicException)
                    {
                        _logger.LogError("The client secret could not be decrypted. It may have been encrypted using a different key.");
                        model.ClientSecret = string.Empty;
                        model.HasDecryptionError = true;
                    }
                }
                else
                {
                    model.ClientSecret = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(settings.AppSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(WechatConstants.Features.WechatAuthentication);
                        model.AppSecret = protector.Unprotect(settings.AppSecret);
                    }
                    catch (CryptographicException)
                    {
                        _logger.LogError("The app secret could not be decrypted. It may have been encrypted using a different key.");
                        model.AppSecret = string.Empty;
                        model.HasDecryptionError = true;
                    }
                }
                else
                {
                    model.AppSecret = string.Empty;
                }

                if (settings.CallbackPath.HasValue)
                {
                    model.CallbackUrl = settings.CallbackPath.Value;
                }
                model.SaveTokens = settings.SaveTokens;
                model.EnableOfficalAccount = settings.EnableOfficialAccount;
            }).Location("Content:5").OnGroup(WechatConstants.Features.WechatAuthentication);
        }

        public override async Task<IDisplayResult> UpdateAsync(WechatAuthenticationSettings settings, BuildEditorContext context)
        {
            if (context.GroupId == WechatConstants.Features.WechatAuthentication)
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (!await _authorizationService.AuthorizeAsync(user, Permissions.ManageWechatAuthentication))
                {
                    return null;
                }

                var model = new WechatAuthenticationSettingsViewModel();
                await context.Updater.TryUpdateModelAsync(model, Prefix);

                if (context.Updater.ModelState.IsValid)
                {
                    var protector = _dataProtectionProvider.CreateProtector(WechatConstants.Features.WechatAuthentication);

                    settings.ClientID = model.ClientID;
                    settings.ClientSecret = protector.Protect(model.ClientSecret);
                    settings.CallbackPath = model.CallbackUrl;
                    settings.SaveTokens = model.SaveTokens;
                    settings.AppId = model.AppId;
                    settings.AppSecret = protector.Protect(model.AppSecret);
                    settings.EnableOfficialAccount = model.EnableOfficalAccount;
                    await _shellHost.ReleaseShellContextAsync(_shellSettings);
                }
            }
            return await EditAsync(settings, context);
        }
    }
}
