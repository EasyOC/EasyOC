using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using OrchardCore.Wechat.Settings;

namespace OrchardCore.Wechat.Services
{
    public interface IWechatAuthenticationService
    {
        Task<WechatAuthenticationSettings> GetSettingsAsync();
        Task<WechatAuthenticationSettings> LoadSettingsAsync();
        Task UpdateSettingsAsync(WechatAuthenticationSettings settings);
        IEnumerable<ValidationResult> ValidateSettings(WechatAuthenticationSettings settings);
    }
}
