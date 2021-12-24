using EasyOC.WebApi.Dto;
using OrchardCore.DisplayManagement;
using OrchardCore.Navigation;
using OrchardCore.Users.Models;
using OrchardCore.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.WebApi.Services
{
     public interface IUserAppService
    {
        Task<PagedResultDto<UserDto>> GetAllAsync(UserIndexOptionsDto options, PagerParameters pagerParameters);
        Task BulkActionAsync(UserIndexOptionsDto options, IEnumerable<string> itemIds);
        Task CreateUserAsync(UserDto user);
        Task<UserDto> GetUserAsync(string id);
        Task UpdateAsync(UserDto userDto);
        Task DeleteAsync(string id);
        Task EditPasswordAsync(ResetUserPasswordtInput model);

    }
}
