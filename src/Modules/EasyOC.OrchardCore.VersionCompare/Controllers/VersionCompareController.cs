using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Contents;
using OrchardCore.Contents.Deployment.Download;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.VersionCompare.Controllers
{
    public class VersionCompareController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentManager _contentManager;

        public VersionCompareController(
            IAuthorizationService authorizationService,
            IContentManager contentManager
            )
        {
            _authorizationService = authorizationService;
            _contentManager = contentManager;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CompareContent(string contentItemId, string sourceVersion = "", string targetVersion = "")
        {
            var latest = string.IsNullOrEmpty(sourceVersion);
            var contentItem = await _contentManager.GetAsync
                (contentItemId, latest == false ? VersionOptions.Published : VersionOptions.AllVersions);

            if (contentItem == null)
            {
                return NotFound();
            }

            // Export permission is required as the overriding permission.
            // Requesting EditContent would allow custom permissions to deny access to this content item.
            if (!await _authorizationService.AuthorizeAsync(User, CommonPermissions.EditContent, contentItem))
            {
                return Forbid();
            }

            var model = new DisplayJsonContentItemViewModel
            {
                ContentItem = contentItem,
                ContentItemJson = JObject.FromObject(contentItem).ToString()
            };

            return View(model);
        }
    }
}



