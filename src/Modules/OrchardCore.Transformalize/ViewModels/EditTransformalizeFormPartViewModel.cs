using OrchardCore.ContentFields.Fields;
using TransformalizeModule.Models;

namespace TransformalizeModule.ViewModels {
   public class EditTransformalizeFormPartViewModel {
      public TransformalizeFormPart TransformalizeFormPart { get; set; }
      public TextField Arrangement { get; set; }
      public BooleanField LocationEnableHighAccuracy { get; set; }
      public NumericField LocationMaximumAge { get; set; }
      public NumericField LocationTimeout { get; set; }
      public string CreateCommand { get; set; }
      public string InsertCommand { get; set; }
      public string UpdateCommand { get; set; }
      public string SelectCommand { get; set; }
   }
}