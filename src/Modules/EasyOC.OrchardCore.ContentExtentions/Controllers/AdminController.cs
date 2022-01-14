using EasyOC.OrchardCore.ContentExtentions.AppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentTypes;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.ContentExtentions.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentManagementAppService _contentManagementAppService;

        public AdminController(IContentManagementAppService contentManagementAppService, IAuthorizationService authorizationService)
        {
            _contentManagementAppService = contentManagementAppService;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> GetTypeDefinitionAsync(string name, bool withSettings = false)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditContentTypes))
            {
                return Forbid();
            }
            return Json(_contentManagementAppService.GetTypeDefinition(name, withSettings));
        }
    }
}



