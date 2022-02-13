using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;
using TransformalizeModule.Activities;
using TransformalizeModule.ViewModels;

namespace TransformalizeModule.Drivers {
   public class TransformalizeActivityDisplayDriver : ActivityDisplayDriver<TransformalizeActivity, TransformalizeActivityViewModel> {
      protected override void EditActivity(TransformalizeActivity activity, TransformalizeActivityViewModel model) {
         model.AliasExpression = activity.Alias.Expression;
      }

      protected override void UpdateActivity(TransformalizeActivityViewModel model, TransformalizeActivity activity) {
         activity.Alias = new WorkflowExpression<string>(model.AliasExpression);
      }
   }
}
