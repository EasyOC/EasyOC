using EasyOC.Workflows.Activities;
using EasyOC.Workflows.ViewModels;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace EasyOC.Workflows.Drivers
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



