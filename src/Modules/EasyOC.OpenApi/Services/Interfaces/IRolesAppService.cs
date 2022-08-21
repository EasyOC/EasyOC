using EasyOC.OpenApi.Dto;
using OrchardCore.Security;
using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OpenApi.Services
{
    public interface IRolesAppService
    {
        Task CreateRoleAsync(RoleDto model);
        Task DeleteRoleAsync(string id);
        Task<IDictionary<string, IEnumerable<PermissionDto>>> GetAllPermissionsAsync();
        Task<RoleDetailsDto> GetRoleDetailsAsync(string id);
        Task<List<RoleDto>> GetRolesAsync();
        Task UpdateRoleAsync(RoleDetailsDto input);
        Task<IEnumerable<string>> GetEffectivePermissions(Role role, IEnumerable<Permission> allPermissions);
        Task<IDictionary<string, IEnumerable<Permission>>> GetInstalledPermissionsAsync();
    }
}
