using EasyOC.OrchardCore.OpenApi.Dto;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Services
{
    public interface IUsersAppService
    {
        Task<PagedResultDto<UserDto>> GetAllAsync(GetAllUserInput input);
        Task BulkActionAsync(UsersBulkActionInput BulkAction);
        Task CreateUserAsync(UserDto user);
        Task<UserDto> GetUserAsync(string id);
        Task UpdateAsync(UserDto userDto);Task DeleteAsync(string id);
        Task EditPasswordAsync(ResetUserPasswordtInput model);

    }
}
