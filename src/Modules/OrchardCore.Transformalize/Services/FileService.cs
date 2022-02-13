using OrchardCore.ContentManagement;
using System.Threading.Tasks;
using TransformalizeModule.Models;
using TransformalizeModule.Services.Contracts;

namespace TransformalizeModule.Services {
   public class FileService : IFileService {

      private IContentManager _contentManager;
      public FileService(IContentManager contentManager) {
         _contentManager = contentManager;
      }
      public async Task<ContentItem> GetFileItem(string id) {
         if (!string.IsNullOrEmpty(id)) {
            return await _contentManager.GetAsync(id);
         }
         return null;
      }

      public async Task<TransformalizeFilePart> GetFilePart(string id) {
         var contentItem = await GetFileItem(id);
         if (contentItem != null) {
            return contentItem.As<TransformalizeFilePart>();
         }
         return null;
      }
   }
   
}
