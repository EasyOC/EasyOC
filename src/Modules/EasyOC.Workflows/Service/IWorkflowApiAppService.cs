using EasyOC.Workflows.Models;
using System.Collections.Generic;

namespace EasyOC.Workflows.Service
{
    public interface IWorkflowApiAppService
    {
        IEnumerable<GlobalMethodDto> ListAllGlobalMethods();
    }
}


