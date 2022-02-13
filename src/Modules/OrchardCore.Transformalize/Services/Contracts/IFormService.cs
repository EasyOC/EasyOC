using TransformalizeModule.Models;
using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using Transformalize.Configuration;
using System.Collections.Generic;

namespace TransformalizeModule.Services.Contracts {

   public interface IFormService : IArrangementService, IArrangementRunService {
      Task<TransformalizeResponse<TransformalizeTaskPart>> ValidateParameters(TransformalizeRequest request);
      Process LoadForParameters(ContentItem contentItem, IDictionary<string, string> parameters = null);

      Task<TransformalizeResponse<TransformalizeFormPart>> ValidateForm(TransformalizeRequest request);
      Process LoadForForm(ContentItem contentItem, IDictionary<string, string> parameters = null, string format = null);
   }

}
