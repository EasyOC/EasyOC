using EasyOC.OrchardCore.OpenApi.Dto;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.Users.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Services
{
    public interface IUsersAppService
    {
        Task<PagedResult<UserListItemDto>> GetAllAsync(GetAllUserInput input);
        Task BulkActionAsync(UsersBulkActionInput BulkAction);
        Task CreateUserAsync(UserDto user);
        Task<UserDto> GetUserAsync(string id);
        Task UpdateAsync(UserDto userDto); Task DeleteAsync(string id);
        Task EditPasswordAsync(ResetUserPasswordtInput model);
        IEnumerable<ContentTypeDefinitionDto> GetUserSettingsTypes();
        IEnumerable<ContentTypeDefinition> GetUserSettingsTypeDefinitions();
        Task<ContentItem> GetUserSettingsAsync(User user, string settingsTypeName);
        //Task<ContentItemDto> GetUserSettingsAsync(string userId, string settingsTypeName);

    }
}
