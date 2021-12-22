using System.ComponentModel.DataAnnotations;

namespace EasyOC.OrchardCore.WorkflowPlus.ViewModels
{
    public class EmailPlusViewModel
    {
        public string AuthorExpression { get; set; }

        public string SenderExpression { get; set; }
        [Required]
        public string ToExpression { get; set; }
        public string CcExpression { get; set; }
        public string BccExpression { get; set; }
        public string ReplyToExpression { get; set; }
        public string SubjectExpression { get; set; }
        public string Body { get; set; }
        public string AttachmentUrl { get; set; }
        public string AttachmentName { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}



