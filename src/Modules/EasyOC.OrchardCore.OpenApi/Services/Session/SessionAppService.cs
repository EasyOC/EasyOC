using EasyOC.Core.Application;
using EasyOC.OrchardCore.OpenApi.Dto;
using EasyOC.OrchardCore.OpenApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Security;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.OpenApi.Services.Session
{
    [EOCAuthorization]
    public class SessionAppService : AppServcieBase
    {
        private readonly IRolesAppService _rolesAppService;
        private readonly RoleManager<IRole> _roleManager;
        private readonly IUsersAppService _usersAppService;
        public SessionAppService(IRolesAppService rolesAppService, IUsersAppService usersAppService, RoleManager<IRole> roleManager)
        {
            _rolesAppService = rolesAppService;
            _usersAppService = usersAppService;
            _roleManager = roleManager;
        }

        public async Task<UserDetailsDto> GetCurrentUserInfoAsync()
        {

            var user = await UserManager.FindByNameAsync(HttpContextAccessor.HttpContext.User.Identity.Name) as User;
            if (user == null)
            {
                throw new AppFriendlyException("User not found.", StatusCodes.Status403Forbidden);
            }
            return ObjectMapper.Map<UserDetailsDto>(user);
        }

        public async Task<IEnumerable<string>> MenusAsync()
        {
            var user = await CurrentUserAsync;
            var userDetails = user as User;
            var userPermissions = new List<string>();
            var allMenus = YesSession.Query<ContentItem, ContentItemIndex>().Where(x => x.ContentType == "VbenMenu" && x.Published && x.Latest);
             
           

            return userPermissions;
        }

    }
}
