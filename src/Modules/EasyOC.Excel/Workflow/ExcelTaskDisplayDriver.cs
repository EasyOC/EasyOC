using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace EasyOC.Excel.Workflow
{
    public class ExcelTaskDisplayDriver : ActivityDisplayDriver<ExcelTask, ExcelTaskViewModel>
    {
        protected override void EditActivity(ExcelTask activity, ExcelTaskViewModel model)
        {
            model.RowFilter = activity.RowFilter.Expression;
            model.FilePath = activity.FilePath;
            model.PropertyName = activity.PropertyName;
            model.ExtraScripts = activity.ExtraScripts.Expression;
            model.FromUpload = activity.FromUpload;

        }

        protected override void UpdateActivity(ExcelTaskViewModel model, ExcelTask activity)
        {
            activity.RowFilter = new WorkflowExpression<string>(model.RowFilter);
            activity.FilePath = model.FilePath;
            activity.FromUpload = model.FromUpload;
            activity.PropertyName = model.PropertyName;
            activity.ExtraScripts = new WorkflowExpression<object>(model.ExtraScripts);

        }
    }
}



