using EasyOC.Core.Application;
using EasyOC.OrchardCore.OpenApi.Dto;
using Microsoft.AspNetCore.Http;
using OrchardCore.Users.Models;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Services.UserInfo
{
    [EOCAuthorization]
    public class UserInfoAppService : AppServcieBase, IUserInfoAppService
    {

        public async Task<UserDetailsDto> GetUserInfoAsync()
        {

            var user = await UserManager.FindByNameAsync(HttpContextAccessor.HttpContext.User.Identity.Name) as User;
            if (user == null)
            {
                throw new AppFriendlyException("User not found.", StatusCodes.Status403Forbidden);
            }
            return ObjectMapper.Map<UserDetailsDto>(user);
        }
        public async Task<UserDetailsDto> GetUserMenuListAsync(string userName)
        {
            return null;
        }
    }
}
