using TransformalizeModule.Models;
using TransformalizeModule.Services.Contracts;
using OrchardCore.ContentManagement;
using System.Threading.Tasks;
using Transformalize.Configuration;

namespace TransformalizeModule.Services {
   public class CommonService : ICommonService {

      private readonly IArrangementService _arrangementService;
      private readonly IArrangementLoadService _loadService;

      public CommonService(
         IArrangementService arrangementService, 
         IArrangementLoadService loadService
      ) {
         _arrangementService = arrangementService;
         _loadService = loadService;
      }

      public bool CanAccess(ContentItem contentItem) {
         return _arrangementService.CanAccess(contentItem);
      }

      public Task<ContentItem> GetByIdOrAliasAsync(string idOrAlias) {
         return _arrangementService.GetByIdOrAliasAsync(idOrAlias);
      }

      public Process LoadForSchema(ContentItem contentItem, string format) {
         return _loadService.LoadForSchema(contentItem, format);
      }

      public async Task<TransformalizeResponse<ContentPart>> Validate(TransformalizeRequest request) {

         var response = new TransformalizeResponse<ContentPart>(request.Format) {
            ContentItem = await GetByIdOrAliasAsync(request.ContentItemId)
         };

         if (response.ContentItem == null) {
            SetupNotFoundResponse(request, response);
            return response;
         }

         if (request.Secure && !CanAccess(response.ContentItem)) {
            SetupPermissionsResponse(request, response);
            return response;
         }

         // part and process left null

         response.Valid = true;
         return response;
      }

      public void SetupInvalidParametersResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response) {
         _arrangementService.SetupInvalidParametersResponse(request, response);
      }

      public void SetupPermissionsResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response) {
         _arrangementService.SetupPermissionsResponse(request, response);
      }

      public void SetupNotFoundResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response) {
         _arrangementService.SetupNotFoundResponse(request, response);
      }
      public void SetupLoadErrorResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response) {
         _arrangementService.SetupLoadErrorResponse(request, response);
      }

      public void SetupWrongTypeResponse<T1>(TransformalizeRequest request, TransformalizeResponse<T1> response) {
         _arrangementService.SetupWrongTypeResponse(request, response);
      }

      public void SetupCustomErrorResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response, string error) {
         _arrangementService.SetupCustomErrorResponse(request, response, error);
      }
   }
}
