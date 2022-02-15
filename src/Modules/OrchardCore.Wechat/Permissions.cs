using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Security.Permissions;

namespace OrchardCore.Wechat
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageWechatAuthentication
            = new Permission(nameof(ManageWechatAuthentication), "Manage Wechat Authentication settings");

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[] { ManageWechatAuthentication }.AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            yield return new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = new[]
                {
                    ManageWechatAuthentication
                }
            };
        }
    }
}
