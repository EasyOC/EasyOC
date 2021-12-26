using EasyOC.WebApi.Dto;
using OrchardCore.Roles.ViewModels;
using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.WebApi.Services
{
    public interface IRolesAppService
    {
        Task CreateRoleAsync(RoleDto model);
        Task DeleteRoleAsync(string id);
        Task<IDictionary<string, IEnumerable<PermissionDto>>> GetAllPermissionsAsync();
        Task<RoleDetailsDto> GetRoleDetailsAsync(string id);
        Task<List<RoleDto>> GetRolesAsync();
        Task UpdateRoleAsync(UpdateRoleInput input);
    }
}
