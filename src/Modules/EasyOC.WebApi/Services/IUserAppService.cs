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
        Task<PagedResultDto<User>> Index(UserIndexOptions options, PagerParameters pagerParameters);
        Task BulkAction(UserIndexOptions options, IEnumerable<string> itemIds);
        Task<IShape> Create();
        Task CreatePost(User user);
        Task<User> GetUserAsync(string id);
        Task EditPost(string id, string returnUrl);
        Task Delete(string id);
        Task EditPassword(string id);
        Task EditPassword(UserDto model);

    }
}
