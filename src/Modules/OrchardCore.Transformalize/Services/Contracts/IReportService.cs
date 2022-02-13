using TransformalizeModule.Models;
using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using Transformalize.Configuration;

namespace TransformalizeModule.Services.Contracts {
   public interface IReportService : IArrangementService, IArrangementStreamService {
      Task<TransformalizeResponse<TransformalizeReportPart>> Validate(TransformalizeRequest request);

      Process LoadForReport(ContentItem contentItem, string format = null);
      Process LoadForStream(ContentItem contentItem);
      Process LoadForBatch(ContentItem contentItem);
      Process LoadForMap(ContentItem contentItem);
      Process LoadForMapStream(ContentItem contentItem);
   }
}
