using EasyOC.OrchardCore.WorkflowPlus.Activities;
using EasyOC.OrchardCore.WorkflowPlus.ViewModels;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace EasyOC.OrchardCore.WorkflowPlus.Drivers
{
    public class WorkflowFaultEventDisplayDriver : ActivityDisplayDriver<WorkflowFaultEvent, WorkflowFaultViewModel>
    {
        protected override void EditActivity(WorkflowFaultEvent activity, WorkflowFaultViewModel model)
        {
            model.ErrorFilter = activity.ErrorFilter.Expression;

        }
        protected override void UpdateActivity(WorkflowFaultViewModel model, WorkflowFaultEvent activity)
        {
            activity.ErrorFilter = new WorkflowExpression<bool>(model.ErrorFilter);

        }
    }
}



