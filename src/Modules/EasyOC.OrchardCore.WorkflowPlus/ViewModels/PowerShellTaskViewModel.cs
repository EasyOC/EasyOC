using System.ComponentModel.DataAnnotations;

namespace EasyOC.OrchardCore.WorkflowPlus.ViewModels
{
    public class PowerShellTaskViewModel
    {
        [Required]
        public string ScriptText { get; set; }

        public string PropertyName { get; set; }
        public bool UseJavascript { get; set; }
    }
}


