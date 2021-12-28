using EasyOC.OrchardCore.OpenApi.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Services
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
