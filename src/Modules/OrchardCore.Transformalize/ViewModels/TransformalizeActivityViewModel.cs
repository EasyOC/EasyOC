using System.ComponentModel.DataAnnotations;

namespace TransformalizeModule.ViewModels {
   public class TransformalizeActivityViewModel {
      [Required]
      public string AliasExpression { get; set; }
   }
}
