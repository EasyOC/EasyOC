using EasyOC.OrchardCore.Workflows.Activities;
using EasyOC.OrchardCore.Workflows.ViewModels;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.Workflow.Drivers
{
    public class CAPEventDisplayDriver : ActivityDisplayDriver<CAPEvent, CAPEventViewModel>
    {

        protected override void EditActivity(CAPEvent activity, CAPEventViewModel model)
        {
            model.SubScribeKey = activity.SubScribeKey.Expression;
        }
        protected override void UpdateActivity(CAPEventViewModel model, CAPEvent activity)
        {
            activity.SubScribeKey = new WorkflowExpression<string>(model.SubScribeKey);
        }
    }
}



