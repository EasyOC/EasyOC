using EasyOC.OrchardCore.WorkflowPlus.Activities;
using EasyOC.OrchardCore.WorkflowPlus.ViewModels;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace EasyOC.OrchardCore.WorkflowPlus.Drivers
{
    public class CreateUserTaskDriver : ActivityDisplayDriver<CreateUserTask, CreateUserTaskViewModel>
    {
        protected override void EditActivity(CreateUserTask activity, CreateUserTaskViewModel model)
        {
            model.Script = activity.Script.Expression;
        }

        protected override void UpdateActivity(CreateUserTaskViewModel model, CreateUserTask activity)
        {
            activity.Script = new WorkflowExpression<string>(model.Script);
        }
    }
}



