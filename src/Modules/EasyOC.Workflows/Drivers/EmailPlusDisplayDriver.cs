using EasyOC.Workflows.Activities;
using EasyOC.Workflows.ViewModels;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;

namespace EasyOC.Workflows.Drivers
{
    public class EmailPlusDisplayDriver : ActivityDisplayDriver<EmailPlus, EmailPlusViewModel>
    {
        protected override void EditActivity(EmailPlus activity, EmailPlusViewModel model)
        {
            model.SenderExpression = activity.From.Expression;
            model.AuthorExpression = activity.Author.Expression;
            model.ToExpression = activity.To.Expression;
            model.BccExpression = activity.BCC.Expression;
            model.CcExpression = activity.CC.Expression;
            model.ReplyToExpression = activity.ReplyTo.Expression;
            model.SubjectExpression = activity.Subject.Expression;
            model.Body = activity.Body.Expression;
            model.AttachmentUrl = activity.AttachmentUrl.Expression;
            model.AttachmentName = activity.AttachmentName.Expression;
            model.IsBodyHtml = activity.IsBodyHtml;
        }

        protected override void UpdateActivity(EmailPlusViewModel model, EmailPlus activity)
        {
            activity.From = new WorkflowExpression<string>(model.SenderExpression);
            activity.Author = new WorkflowExpression<string>(model.AuthorExpression);
            activity.To = new WorkflowExpression<string>(model.ToExpression);
            activity.CC = new WorkflowExpression<string>(model.CcExpression);
            activity.BCC = new WorkflowExpression<string>(model.BccExpression);
            activity.ReplyTo = new WorkflowExpression<string>(model.ReplyToExpression);
            activity.Subject = new WorkflowExpression<string>(model.SubjectExpression);
            activity.Body = new WorkflowExpression<string>(model.Body);
            activity.AttachmentUrl = new WorkflowExpression<string>(model.AttachmentUrl);
            activity.AttachmentName = new WorkflowExpression<string>(model.AttachmentName);
            activity.IsBodyHtml = model.IsBodyHtml;
        }
    }
}



