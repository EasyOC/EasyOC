using TransformalizeModule.Models;
using TransformalizeModule.Services.Contracts;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transformalize.Configuration;

namespace TransformalizeModule.Services {
   public class TaskService : ITaskService {

      private readonly IArrangementService _arrangementService;
      private readonly IArrangementLoadService _loadService;
      private readonly IArrangementRunService _runService;

      public TaskService(
         IArrangementService arrangementService, 
         IArrangementLoadService loadService,
         IArrangementRunService runService
      ) {
         _arrangementService = arrangementService;
         _loadService = loadService;
         _runService = runService;
      }

      public bool CanAccess(ContentItem contentItem) {
         return _arrangementService.CanAccess(contentItem);
      }

      public Task<ContentItem> GetByIdOrAliasAsync(string idOrAlias) {
         return _arrangementService.GetByIdOrAliasAsync(idOrAlias);
      }

      public Process LoadForTask(ContentItem contentItem, IDictionary<string,string> parameters = null, string format = null) {
         return _loadService.LoadForTask(contentItem, parameters, format);
      }

      public async Task RunAsync(Process process) {
         await _runService.RunAsync(process);
      }

      public void Run(Process process) {
         _runService.Run(process);
      }

      public async Task<TransformalizeResponse<TransformalizeTaskPart>> Validate(TransformalizeRequest request) {

         var response = new TransformalizeResponse<TransformalizeTaskPart>(request.Format) {
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

         response.Part = response.ContentItem.As<TransformalizeTaskPart>();
         if (response.Part == null) {
            SetupWrongTypeResponse(request, response);
            return response;
         }

         response.Process = LoadForTask(response.ContentItem, request.InternalParameters, request.Format);
         if (response.Process.Status != 200) {
            SetupLoadErrorResponse(request, response);
            return response;
         }

         if (request.ValidateParameters && !response.Process.Parameters.All(p => p.Valid)) {
            SetupInvalidParametersResponse(request, response);
            return response;
         }

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
