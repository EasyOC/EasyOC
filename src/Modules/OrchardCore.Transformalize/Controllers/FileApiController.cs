using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TransformalizeModule.Services.Contracts;
using TransformalizeModule.Services;
using TransformalizeModule.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using OrchardCore.ContentManagement;

namespace TransformalizeModule.Controllers {

   [Route("t/api/file")]
   [ApiController]
   [Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
   public class FileApiController : Controller {

      private readonly CombinedLogger<FileApiController> _logger;
      private readonly ICustomFileStore _formFileStore;
      private readonly IContentManager _contentManager;
      private readonly IAuthorizationService _authorizationService;

      public FileApiController(
         ICustomFileStore formFileStore,
         IContentManager contentManager,
         CombinedLogger<FileApiController> logger,
         IAuthorizationService authorizationService
      ) {
         _logger = logger;
         _formFileStore = formFileStore;
         _contentManager = contentManager;
         _authorizationService = authorizationService;
      }

      [HttpPost]
      [Route("upload")]
      [RequestSizeLimit(209_715_200)]
      public async Task<IActionResult> Upload() {

         if (!await _authorizationService.AuthorizeAsync(User, Permissions.AllowApi)) {
            return GetResult(string.Empty, "Unauthorized");
         }

         // todo extract this to service
         if (Request.HasFormContentType && Request.Form.Files != null && Request.Form.Files.Count > 0) {
            var file = Request.Form.Files[0];
            if (file != null && file.Length > 0) {

               var contentItem = await _contentManager.NewAsync("TransformalizeFile");
               var part = contentItem.As<TransformalizeFilePart>();
               part.OriginalName.Text = file.FileName;

               var filePath = Path.Combine(Common.GetSafeFilePath(part, HttpContext.User.Identity.Name));

               using (var stream = file.OpenReadStream()) {
                  await _formFileStore.CreateFileFromStreamAsync(filePath, stream, true);
               }

               var fileInfo = await _formFileStore.GetFileInfoAsync(filePath);

               part.FullPath.Text = fileInfo.Path;

               contentItem.Apply(part);

               await _contentManager.CreateAsync(contentItem);

               return GetResult(contentItem.ContentItemId, file.FileName);
            }
         }

         return GetResult(string.Empty, "Error");
      }

      private static ContentResult GetResult(string id, string message) {
         var data = string.Format("{{ \"id\":\"{0}\", \"message\":\"{1}\" }}", id, message);
         return new ContentResult {
            Content = data,
            ContentType = "text/json"
         };
      }

   }
}
