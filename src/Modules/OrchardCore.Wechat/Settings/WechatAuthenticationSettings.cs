using Microsoft.AspNetCore.Http;

namespace OrchardCore.Wechat.Settings
{
    public class WechatAuthenticationSettings
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public PathString CallbackPath { get; set; }
        public bool SaveTokens { get; set; }
        public bool EnableOfficialAccount { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
