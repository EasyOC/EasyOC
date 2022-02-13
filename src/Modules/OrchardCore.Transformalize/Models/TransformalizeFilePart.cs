using OrchardCore.ContentManagement;
using OrchardCore.ContentFields.Fields;
using System.IO;

namespace TransformalizeModule.Models {
   public class TransformalizeFilePart : ContentPart {
      public TransformalizeFilePart() {
         OriginalName = new TextField { Text = string.Empty };
         FullPath = new TextField { Text = string.Empty };
      }

      public TextField OriginalName { get; set; }
      public TextField FullPath { get; set; }

      public string MimeType() {
         return Common.GetMimeType(Extension());
      }

      public string Extension() {
         return Path.GetExtension(OriginalName.Text);
      }

      public bool HasMimeType() {
         return Common.HasMimeType(Extension());
      }
   }
}
