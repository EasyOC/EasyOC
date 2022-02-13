using OrchardCore.ContentManagement;
using TransformalizeModule.Models;
using System.Threading.Tasks;

namespace TransformalizeModule.Services.Contracts {
   public interface ICommonService : IArrangementService {
      Task<TransformalizeResponse<ContentPart>> Validate(TransformalizeRequest request);
   }

}
