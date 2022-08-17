using System.ComponentModel.DataAnnotations;

namespace EasyOC.Workflows.ViewModels
{
    public class WorkflowFaultViewModel
    {
        [Required]
        public string ErrorFilter { get; set; }
    }
}


