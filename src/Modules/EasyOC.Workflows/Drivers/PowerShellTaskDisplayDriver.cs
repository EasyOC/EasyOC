using EasyOC.Workflows.Activities;
using EasyOC.Workflows.ViewModels;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace EasyOC.Workflows.Drivers
{
    public class PowerShellTaskDisplayDriver : ActivityDisplayDriver<PowerShellTask, PowerShellTaskViewModel>
    {

        protected override void EditActivity(PowerShellTask activity, PowerShellTaskViewModel model)
        {
            model.PropertyName = activity.PropertyName;
            model.ScriptText = activity.ScriptText.Expression;
            model.UseJavascript = activity.UseJavascript;
        }
        protected override void UpdateActivity(PowerShellTaskViewModel model, PowerShellTask activity)
        {
            activity.ScriptText = new WorkflowExpression<string>(model.ScriptText);
            activity.PropertyName = model.PropertyName;
            activity.UseJavascript = model.UseJavascript;
        }
    }
}



