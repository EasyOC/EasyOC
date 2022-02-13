using OrchardCore.ContentManagement;
using System.Threading.Tasks;
using TransformalizeModule.Models;

namespace TransformalizeModule.Services.Contracts {
   public interface IFileService {
      Task<ContentItem> GetFileItem(string id);
      Task<TransformalizeFilePart> GetFilePart(string id);
   }

}
