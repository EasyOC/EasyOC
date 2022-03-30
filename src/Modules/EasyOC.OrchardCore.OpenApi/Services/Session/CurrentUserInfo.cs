using EasyOC.Core.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Services.Session
{

    public class CurrentUserInfo : AppServcieBase
    {
        private readonly IRolesAppService _rolesAppService;
        private readonly IUsersAppService _usersAppService;
        public CurrentUserInfo(IRolesAppService rolesAppService, IUsersAppService usersAppService = null)
        {
            _rolesAppService = rolesAppService;
            _usersAppService = usersAppService;
        }

        public async Task<IEnumerable<string>> Permissions()
        {
            var user = await CurrentUserAsync;
            UserManager.get
            return Task.FromResult(new[] { string.Empty }.AsEnumerable());
        }


    }
}
