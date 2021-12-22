using Microsoft.AspNetCore.Mvc.Localization;

namespace Lombiq.HelpfulExtensions.Models
{
    public class BootstrapAccordionItem
    {
        public LocalizedHtmlString Title { get; set; }
        public dynamic Shape { get; set; }
    }
}
