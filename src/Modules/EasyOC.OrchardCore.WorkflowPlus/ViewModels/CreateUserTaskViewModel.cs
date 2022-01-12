using System.ComponentModel.DataAnnotations;

namespace EasyOC.OrchardCore.WorkflowPlus.ViewModels
{
    public class CreateUserTaskViewModel
    {
        [Required]
        public string Script { get; set; } 
    }
}


