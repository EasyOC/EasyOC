using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace TransformalizeModule.Models {
   public class TransformalizeTaskPart : ContentPart {

      public TransformalizeTaskPart() {
         Arrangement = new TextField();
      }
      public TextField Arrangement { get; set; }
   }
}
