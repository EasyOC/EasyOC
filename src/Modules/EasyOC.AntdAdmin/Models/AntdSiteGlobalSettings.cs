using AutoMapper;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace EasyOC.AntdAdmin.Models
{
    [AutoMap(typeof(AntdSiteGlobalSettingsDto), ReverseMap = true)]
    public class AntdSiteGlobalSettings : ContentPart
    {
        public TextField MenuData { get; set; }
        public TextField SettingData { get; set; }
        // public string SiteDescription { get; set; }
        // public string SiteKeywords { get; set; }
        // public string SiteLogo { get; set; }
        // public string SiteIcon { get; set; }
        // public string SiteCopyright { get; set; }
        // public string SiteTheme { get; set; }
        // public string SiteLanguage { get; set; }
        // public string SiteTimezone { get; set; }
        // public string SiteCurrency { get; set; }
    }

    public class AntdSiteGlobalSettingsDto
    {
        public string MenuData { get; set; }
        public string SiteSettingsData { get; set; }
    }

}
