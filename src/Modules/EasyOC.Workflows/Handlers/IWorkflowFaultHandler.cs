using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System;
using System.Threading.Tasks;

namespace EasyOC.Workflows.Servcie
{
    public interface IWorkflowFaultHandler
    {
        Task OnWorkflowFaultAsync(IWorkflowManager workflowManager,
            WorkflowExecutionContext workflowContext,
            ActivityContext activityContext,
            Exception exception);
    }
}


