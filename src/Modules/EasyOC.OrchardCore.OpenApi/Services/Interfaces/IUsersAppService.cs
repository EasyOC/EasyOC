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
        Task NewUserAsync(UserDetailsDto user);
        Task<UserDetailsDto> GetUserAsync(string id);
        Task UpdateAsync(UserDetailsDto userDto);
        Task DeleteAsync(string id);
        Task EditPasswordAsync(ResetUserPasswordtInput model);
        IEnumerable<ContentTypeDefinitionDto> GetUserSettingTypes();
        IEnumerable<ContentTypeDefinition> GetUserSettingsTypeDefinitions();
        Task<ContentItem> GetUserSettingsAsync(User user, string settingsTypeName);
        //Task<ContentItemDto> GetUserSettingsAsync(string userId, string settingsTypeName);

    }
}
