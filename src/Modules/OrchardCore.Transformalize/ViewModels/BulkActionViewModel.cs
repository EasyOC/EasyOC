using TransformalizeModule.Models;
using Transformalize.Configuration;

namespace TransformalizeModule.ViewModels {
   public class BulkActionViewModel {
      public Process Summary { get; set; }
      public TransformalizeResponse<TransformalizeTaskPart> Task { get; set; }
      public BulkActionViewModel(TransformalizeResponse<TransformalizeTaskPart> task, Process summary) {
         Summary = summary;
         Task = task;
      }
   }
}
