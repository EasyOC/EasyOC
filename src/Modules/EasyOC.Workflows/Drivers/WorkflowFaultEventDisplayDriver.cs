using EasyOC.Workflows.Activities;
using EasyOC.Workflows.ViewModels;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace EasyOC.Workflows.Drivers
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



