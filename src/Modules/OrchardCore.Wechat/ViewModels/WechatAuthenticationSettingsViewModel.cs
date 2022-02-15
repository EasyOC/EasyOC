using System.ComponentModel.DataAnnotations;

namespace OrchardCore.Wechat.ViewModels
{
    public class WechatAuthenticationSettingsViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Client Id is required")]
        public string ClientID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Client Secret is required")]
        public string ClientSecret { get; set; }

        [RegularExpression(@"\/[-A-Za-z0-9+&@#\/%?=~_|!:,.;]+[-A-Za-z0-9+&@#\/%=~_|]", ErrorMessage = "Invalid path")]
        public string CallbackUrl { get; set; }

        public bool SaveTokens { get; set; }

        public bool HasDecryptionError { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "AppId of Offical Account is required")]
        public string AppId { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "AppSecret of Offical Account is required")]
        public string AppSecret { get; set; }

        public bool EnableOfficalAccount { get; set; }

    }
}
