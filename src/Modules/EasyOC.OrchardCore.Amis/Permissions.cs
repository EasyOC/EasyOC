using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JZSoft.OrchardCore.Amis
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission Amis_Editor = new Permission(nameof(Amis_Editor), "Amis Editor");


        public async Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            var list = new List<Permission> { Amis_Editor };
            return await Task.FromResult(list);
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { Amis_Editor }
                }
            };
        }

    }
}
