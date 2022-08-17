using System.ComponentModel.DataAnnotations;

namespace EasyOC.Workflows.ViewModels
{
    public class CreateUserTaskViewModel
    {
        [Required]
        public string Script { get; set; } 
    }
}


