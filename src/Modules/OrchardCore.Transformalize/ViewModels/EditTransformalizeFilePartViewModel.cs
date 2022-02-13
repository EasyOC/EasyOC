using OrchardCore.ContentFields.Fields;
using TransformalizeModule.Models;

namespace TransformalizeModule.ViewModels {
   public class EditTransformalizeFilePartViewModel {
      public TransformalizeFilePart TransformalizeFilePart { get; set; }
      public TextField OriginalName { get; set; }
      public TextField FullPath { get; set; }
   }
}