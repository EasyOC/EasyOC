using Etch.OrchardCore.ContentPermissions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TransformalizeModule.Models;
using TransformalizeModule.Services.Contracts;
using TransformalizeModule.ViewModels;
using OrchardCore.ContentManagement;
using System.Linq;
using System.Threading.Tasks;

namespace TransformalizeModule.Services {
   public class ArrangementService : IArrangementService {

      private readonly IContentManager _contentManager;
      private readonly IContentHandleManager _aliasManager;
      private readonly IContentPermissionsService _contentPermissionsService;
      private readonly CombinedLogger<ArrangementService> _logger;

      public ArrangementService(
         IContentManager contentManager,
         IContentHandleManager aliasManager, 
         IContentPermissionsService contentPermissionsService,
         CombinedLogger<ArrangementService> logger
         ) {
         _aliasManager = aliasManager;
         _contentManager = contentManager;
         _contentPermissionsService = contentPermissionsService;
         _logger = logger;
      }
      public async Task<ContentItem> GetByIdOrAliasAsync(string idOrAlias) {
         if (string.IsNullOrEmpty(idOrAlias)) {
            return null;
         }

         ContentItem contentItem = null;
         if (idOrAlias.Length == Common.IdLength) {
            contentItem = await _contentManager.GetAsync(idOrAlias);
         }
         if (contentItem == null) {
            var id = await _aliasManager.GetContentItemIdAsync("alias:" + idOrAlias);
            if (id != null) {
               contentItem = await _contentManager.GetAsync(id);
            }
         }
         return contentItem;
      }

      public bool CanAccess(ContentItem contentItem) {
         return _contentPermissionsService.CanAccess(contentItem);
      }

      public void SetupInvalidParametersResponse<T>(TransformalizeRequest request, TransformalizeResponse<T> response) {
         
         response.Process.Status = 422;
         response.Process.Message = Common.InvalidParametersMessage;

         if (request.Format == null) {
            foreach (var parameter in response.Process.Parameters.Where(p => !p.Valid)) {
               foreach (var message in parameter.Message.Split('|')) {
                  _logger.Warn(() => message.Replace("{", "{{").Replace("}", "}}"));
               }
            }
            response.ActionResult = LogResult(response);
         } else {
            response.Process.Connections.Clear();
            response.Process.Log.AddRange(_logger.Log);
            response.ActionResult = ContentResult(request, response);
         }
      }

      public void SetupPermissionsResponse<T>(TransformalizeRequest request, TransformalizeResponse<T> response) {

         _logger.Warn(() => $"User {request.User} may not access {response.ContentItem.DisplayText}.");

         response.Process = new Transformalize.Configuration.Process() { Name = "401", Status = 401, Message = "Unauthorized" };

         if (request.Format == null) {
            response.ActionResult = LogResult(response);
         } else {
            response.ActionResult = ContentResult(request, response);
         }
      }

      public void SetupNotFoundResponse<T>(TransformalizeRequest request, TransformalizeResponse<T> response) {

         _logger.Warn(() => $"User {request.User} requested missing content item {request.ContentItemId}.");

         response.Process.Status = 404;
         response.Process.Message = "Not Found";

         if (request.Format == null) {
            response.ActionResult = LogResult(response);
         } else {
            response.ActionResult = ContentResult(request, response);
         }
      }

      public void SetupLoadErrorResponse<T>(TransformalizeRequest request, TransformalizeResponse<T> response) {

         // process already has a non 200 status

         _logger.Warn(() => $"User {request.User} received error trying to load {response.ContentItem.DisplayText}.");
         
         if (request.Format == null) {
            response.ActionResult = LogResult(response);
         } else {
            response.Process.Connections.Clear();
            response.Process.Log.AddRange(_logger.Log);
            response.ActionResult = ContentResult(request, response);
         }
      }

      public void SetupCustomErrorResponse<T>(TransformalizeRequest request, TransformalizeResponse<T> response, string error) {

         response.Process.Status = 500;
         response.Process.Message = error;

         _logger.Warn(() => $"User {request.User} received error trying to load {response.ContentItem.DisplayText}.");

         if (request.Format == null) {
            response.ActionResult = LogResult(response);
         } else {
            response.Process.Connections.Clear();
            response.Process.Log.AddRange(_logger.Log);
            response.ActionResult = ContentResult(request, response);
         }
      }

      public void SetupWrongTypeResponse<T>(TransformalizeRequest request, TransformalizeResponse<T> response) {

         response.Process.Status = 422;
         response.Process.Message = Common.InvalidContentTypeMessage;

         _logger.Warn(() => $"User {request.User} requested {response.ContentItem.ContentType} from the report service.");
         
         if (request.Format == null) {
            response.ActionResult = LogResult(response);
         } else {
            response.Process.Connections.Clear();
            response.ActionResult = new ContentResult { Content = response.Process.Serialize(), ContentType = request.ContentType };
         }
      }

      public ContentResult ContentResult<T>(TransformalizeRequest request, TransformalizeResponse<T> response) {
         return new ContentResult { Content = response.Process.Serialize(), ContentType = request.ContentType };
      }

      public ViewResult LogResult<T>(TransformalizeResponse<T> response) {
         return View("Log", new LogViewModel(_logger.Log, response.Process, response.ContentItem));
      }

      private ViewResult View(string viewName, object model) {
         return new ViewResult {
            ViewName = viewName,
            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
               Model = model
            }
         };
      }
   }
}
