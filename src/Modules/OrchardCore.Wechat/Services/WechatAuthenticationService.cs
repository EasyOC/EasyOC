using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Entities;
using OrchardCore.Wechat.Settings;
using OrchardCore.Settings;

namespace OrchardCore.Wechat.Services
{
    public class WechatAuthenticationService : IWechatAuthenticationService
    {
        private readonly ISiteService _siteService;
        private readonly IStringLocalizer S;

        public WechatAuthenticationService(
            ISiteService siteService,
            IStringLocalizer<WechatAuthenticationService> stringLocalizer)
        {
            _siteService = siteService;
            S = stringLocalizer;
        }

        public async Task<WechatAuthenticationSettings> GetSettingsAsync()
        {
            var container = await _siteService.GetSiteSettingsAsync();
            return container.As<WechatAuthenticationSettings>();
        }

        public async Task<WechatAuthenticationSettings> LoadSettingsAsync()
        {
            var container = await _siteService.LoadSiteSettingsAsync();
            return container.As<WechatAuthenticationSettings>();
        }

        public async Task UpdateSettingsAsync(WechatAuthenticationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var container = await _siteService.LoadSiteSettingsAsync();
            container.Alter<WechatAuthenticationSettings>(nameof(WechatAuthenticationSettings), aspect =>
            {
                aspect.ClientID = settings.ClientID;
                aspect.ClientSecret = settings.ClientSecret;
                aspect.CallbackPath = settings.CallbackPath;
                aspect.AppId = settings.AppId;
                aspect.AppSecret = settings.AppSecret;
                aspect.EnableOfficialAccount = settings.EnableOfficialAccount;
            });

            await _siteService.UpdateSiteSettingsAsync(container);
        }

        public IEnumerable<ValidationResult> ValidateSettings(WechatAuthenticationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (String.IsNullOrWhiteSpace(settings.ClientID))
            {
                yield return new ValidationResult(S["ClientID is required"], new string[] { nameof(settings.ClientID) });
            }

            if (String.IsNullOrWhiteSpace(settings.ClientSecret))
            {
                yield return new ValidationResult(S["ClientSecret is required"], new string[] { nameof(settings.ClientSecret) });
            }
        }
    }
}
