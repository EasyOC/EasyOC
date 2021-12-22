using System.ComponentModel.DataAnnotations;

namespace EasyOC.OrchardCore.WorkflowPlus.ViewModels
{
    public class WorkflowFaultViewModel
    {
        [Required]
        public string ErrorFilter { get; set; }
    }
}


