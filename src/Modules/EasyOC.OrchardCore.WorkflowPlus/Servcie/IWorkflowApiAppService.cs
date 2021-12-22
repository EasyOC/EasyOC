using EasyOC.OrchardCore.WorkflowPlus.Models;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.WorkflowPlus.Servcie
{
    public interface IWorkflowApiAppService
    {
        IEnumerable<GlobalMethodDto> ListAllGlobalMethods();
    }
}


