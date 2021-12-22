using StatCan.OrchardCore.GCCollab.Settings;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StatCan.OrchardCore.GCCollab.Services
{
    public interface IGCCollabAuthenticationService
    {
        Task<GCCollabAuthenticationSettings> GetSettingsAsync();
        Task UpdateSettingsAsync(GCCollabAuthenticationSettings settings);
        IEnumerable<ValidationResult> ValidateSettings(GCCollabAuthenticationSettings settings);
    }
}



