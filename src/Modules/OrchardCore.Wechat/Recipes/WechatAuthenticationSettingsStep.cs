using System;
using System.Threading.Tasks;
using OrchardCore.Wechat.Services;
using OrchardCore.Wechat.Settings;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;

namespace OrchardCore.Wechat.Recipes
{
    /// <summary>
    /// This recipe step sets Wechat Account settings.
    /// </summary>
    public class WechatAuthenticationSettingsStep : IRecipeStepHandler
    {
        private readonly IWechatAuthenticationService _wechatAuthenticationService;

        public WechatAuthenticationSettingsStep(IWechatAuthenticationService wechatLoginService)
        {
            _wechatAuthenticationService = wechatLoginService;
        }

        public async Task ExecuteAsync(RecipeExecutionContext context)
        {
            if (!String.Equals(context.Name, nameof(WechatAuthenticationSettings), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            var model = context.Step.ToObject<WechatLoginSettingsStepModel>();
            var settings = await _wechatAuthenticationService.LoadSettingsAsync();

            settings.ClientID = model.ConsumerKey;
            settings.ClientSecret = model.ConsumerSecret;
            settings.CallbackPath = model.CallbackPath;

            settings.AppId = model.AppId;
            settings.AppSecret = model.AppSecret;
            settings.EnableOfficialAccount = model.EnableOfficialAccount;

            await _wechatAuthenticationService.UpdateSettingsAsync(settings);
        }
    }

    public class WechatLoginSettingsStepModel
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string CallbackPath { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public bool EnableOfficialAccount { get; set; }

    }
}
