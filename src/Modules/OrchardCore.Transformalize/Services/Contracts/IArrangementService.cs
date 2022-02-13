using OrchardCore.ContentManagement;
using TransformalizeModule.Models;
using System.Threading.Tasks;

namespace TransformalizeModule.Services.Contracts {
   public interface IArrangementService {
      Task<ContentItem> GetByIdOrAliasAsync(string idOrAlias);
      bool CanAccess(ContentItem contentItem);
      void SetupInvalidParametersResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response);
      void SetupPermissionsResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response);
      void SetupNotFoundResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response);
      void SetupLoadErrorResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response);
      void SetupWrongTypeResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response);
      void SetupCustomErrorResponse<TPart>(TransformalizeRequest request, TransformalizeResponse<TPart> response, string error);
   }
}
