using OrchardCore.Workflows.Models;
using System;

namespace EasyOC.Workflows.Models
{
    public class WorkflowStateExt : WorkflowState
    {
        public DateTime LastExecutedOn { get; set; }
    }
}
