using EasyOC.Core.Application;
using EasyOC.OrchardCore.OpenApi.Dto;
using EasyOC.OrchardCore.OpenApi.Indexes;
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
    public class SessionAppService : AppServiceBase
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


        /// <summary>
        /// 返回菜单和权限
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> MenusAsync()
        {
            var user = await UserManager.FindByNameAsync(HttpContextAccessor.HttpContext.User.Identity.Name) as User;

            var userPermissions = new List<string>();
            foreach (var role in user.RoleNames)
            {
                var rolePermissions = await _rolesAppService.GetRoleDetailsAsync(role);
                userPermissions = userPermissions.Union(rolePermissions.Permissions).ToList();

                 //rolePermissions.VbenMenuIds;
            }


            //var menuNames = user.UserClaims.Where(x => x.ClaimType == "VbenMenu").Select(x => x.ClaimValue).ToList();
            //var allMenus = YesSession.Query<ContentItem, ContentItemIndex>().Where(x => x.ContentType == "VbenMenu" && x.Published && x.Latest)
            //    .With<VbenMenuPartIndex>(x => menuNames.Contains(x.MenuName))
            //    .ListAsync();
            //userDetails.UserClaims.Where(x => x.ClaimType == "VbenMenu").ToList().ForEach(x =>
            //{
            //    var permission = x.ClaimValue;
            //    if (allMenus..StartsWith("Menu"))
            //    {
            //        userPermissions.Add(permission);
            //    }
            //});

            //Fsql.Select<ContentItemIndex, VbenMenuPartIndex>().InnerJoin((c, v) =>.).ToList(x => x.t2);

            return userPermissions;

            //return allMenus.to;
        }

    }
}
