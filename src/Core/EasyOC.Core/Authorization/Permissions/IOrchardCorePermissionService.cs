using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.Core.Authorization.Permissions
{
    public interface IOrchardCorePermissionService
    {
        Task<IEnumerable<Permission>> GetPermissionsAsync();
    }
}