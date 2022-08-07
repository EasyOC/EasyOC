using Microsoft.Extensions.Localization;
using OrchardCore.Email;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EasyOC.Workflows.Activities
{
    public class EmailPlus : TaskActivity
    {
        private readonly ISmtpService _smtpService;
        private readonly IWorkflowExpressionEvaluator _expressionEvaluator;
        private readonly IStringLocalizer S;
        private readonly HtmlEncoder _htmlEncoder;

        public EmailPlus(
            ISmtpService smtpService,
            IWorkflowExpressionEvaluator expressionEvaluator,
            IStringLocalizer<EmailPlus> localizer,
            HtmlEncoder htmlEncoder
        )
        {
            _smtpService = smtpService;
            _expressionEvaluator = expressionEvaluator;
            S = localizer;
            _htmlEncoder = htmlEncoder;
        }
        //public override string Name => nameof(EamilPlus);
        //public override LocalizedString DisplayText => S["Email Task"];
        //public override LocalizedString Category => S["Messaging"];
        public override string Name => nameof(EmailPlus);
        public override LocalizedString DisplayText => S["Email Plus"];
        public override LocalizedString Category => S["Messaging"];

        public WorkflowExpression<string> Author
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }
        public WorkflowExpression<string> From
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> To
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> CC
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> BCC
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> ReplyTo
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> Subject
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> Body
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> AttachmentUrl
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> AttachmentName
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public bool IsBodyHtml
        {
            get => GetProperty(() => true);
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(S["Done"], S["Failed"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            var author = await _expressionEvaluator.EvaluateAsync(Author, workflowContext, null);
            var from = await _expressionEvaluator.EvaluateAsync(From, workflowContext, null);
            var to = await _expressionEvaluator.EvaluateAsync(To, workflowContext, null);
            var cc = await _expressionEvaluator.EvaluateAsync(CC, workflowContext, null);
            var bcc = await _expressionEvaluator.EvaluateAsync(BCC, workflowContext, null);
            var replyTo = await _expressionEvaluator.EvaluateAsync(ReplyTo, workflowContext, null);
            var subject = await _expressionEvaluator.EvaluateAsync(Subject, workflowContext, null);
            // Don't html-encode liquid tags if the email is not html
            var body = await _expressionEvaluator.EvaluateAsync(Body, workflowContext, IsBodyHtml ? _htmlEncoder : null);

            var message = new MailMessage
            {
                // Author and Sender are both not required fields.
                From = author?.Trim() ?? from?.Trim(),
                To = to.Trim(),
                Cc = cc?.Trim(),
                Bcc = bcc?.Trim(),
                // Email reply-to header https://tools.ietf.org/html/rfc4021#section-2.1.4
                ReplyTo = replyTo?.Trim(),
                Subject = subject?.Trim(),
                Body = body?.Trim(),
                IsBodyHtml = IsBodyHtml
            };

            if (!string.IsNullOrWhiteSpace(from))
            {
                message.Sender = from.Trim();
            }

            #region 附件
            var attrUrl = await _expressionEvaluator.EvaluateAsync(AttachmentUrl, workflowContext, null);
            var fileName = await _expressionEvaluator.EvaluateAsync(AttachmentName, workflowContext, null);
            if (!string.IsNullOrWhiteSpace(attrUrl) && !string.IsNullOrWhiteSpace(attrUrl))
            {
                try
                {
                    //下载附件
                    var client = new RestClient(attrUrl);
                    var request = new RestRequest(Method.GET);
                    var data = client.DownloadData(request, true);
                    var attachment = new MailMessageAttachment()
                    {
                        Stream = new MemoryStream(data),
                        Filename = fileName
                    };
                    message.Attachments.Add(attachment);
                }
                catch (Exception ex)
                {
                    return Outcomes(ex.Message);
                }
            }
            #endregion
            var result = await _smtpService.SendAsync(message);
            workflowContext.LastResult = result;

            if (!result.Succeeded)
            {
                return Outcomes("Failed");
            }
            return Outcomes("Done");
        }
    }
}



