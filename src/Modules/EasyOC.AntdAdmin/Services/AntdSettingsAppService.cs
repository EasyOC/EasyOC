using EasyOC.Core.Application;
using EasyOC.AntdAdmin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Entities;
using OrchardCore.Settings;
using System;
using System.Threading.Tasks;

namespace EasyOC.AntdAdmin.Services
{
    [EOCAuthorization]
    public class AntdSettingsAppService : AppServiceBase
    {
        private readonly ISiteService _siteService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public AntdSettingsAppService(ISiteService siteService, IContentDefinitionManager contentDefinitionManager)
        {
            _siteService = siteService;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public async Task<AntdSiteGlobalSettingsDto> GetSettings()
        {
            var settings = await _siteService.GetSiteSettingsAsync();
            var content = settings.As<ContentItem>("AntdSiteGlobalSettings");
            var partInfo = content.As<AntdSiteGlobalSettings>();
            var dto = new AntdSiteGlobalSettingsDto
            {
                MenuData = partInfo.MenuData?.Text,
                StaticMenus = partInfo.StaticMenus?.Text,
                SiteSettingsData = partInfo.SettingData?.Text
            };
            return dto;
        }
        public async Task<Object> UpdateSettings(AntdSiteGlobalSettingsDto input)
        {

            var settings = await _siteService.LoadSiteSettingsAsync();

            settings.Alter<ContentItem>("AntdSiteGlobalSettings", content =>
            {
                content.Alter<AntdSiteGlobalSettings>(part =>
                {
                    if (input?.MenuData != null)
                    {
                        part.MenuData.Text = input.MenuData;
                    }
                    if (input?.SiteSettingsData != null)
                    {
                        part.SettingData.Text = input.SiteSettingsData;
                    }
                });
            });
            await _siteService.UpdateSiteSettingsAsync(settings);
            return GetSettings();
        }
    }
}
