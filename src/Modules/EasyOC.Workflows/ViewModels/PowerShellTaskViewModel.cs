using System.ComponentModel.DataAnnotations;

namespace EasyOC.Workflows.ViewModels
{
    public class PowerShellTaskViewModel
    {
        [Required]
        public string ScriptText { get; set; }

        public string PropertyName { get; set; }
        public bool UseJavascript { get; set; }
    }
}


