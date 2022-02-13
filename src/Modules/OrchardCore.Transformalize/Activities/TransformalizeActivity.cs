using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransformalizeModule.Models;
using TransformalizeModule.Services.Contracts;

namespace TransformalizeModule.Activities {

   public class TransformalizeActivity : TaskActivity {

      private readonly IHtmlLocalizer H;
      private readonly IStringLocalizer S;
      private readonly ITaskService _taskService;
      private readonly IWorkflowExpressionEvaluator _expressionEvaluator;

      public TransformalizeActivity(
         ITaskService taskService,
         IStringLocalizer<NotifyTask> s,
         IHtmlLocalizer<NotifyTask> h,
         IWorkflowExpressionEvaluator expressionEvaluator
      ) {
         H = h;
         S = s;
         _taskService = taskService;
         _expressionEvaluator = expressionEvaluator;
      }

      public override string Name => nameof(TransformalizeActivity);

      public override LocalizedString DisplayText => S["Transformalize"];

      public override LocalizedString Category => S["Transformalize"];

      public WorkflowExpression<string> Alias {
         get => GetProperty(() => new WorkflowExpression<string>());
         set => SetProperty(value);
      }

      public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext) {
         return Outcomes(S["Done"], S["Error"]);
      }

      public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext) {

         var alias = await _expressionEvaluator.EvaluateAsync(Alias, workflowContext, null);
         var request = new TransformalizeRequest(alias, null) { Secure = false, InternalParameters = GetParameters(workflowContext) };
         var task = await _taskService.Validate(request);

         if (task.Fails()) {
            workflowContext.Fault(new Exception(task.Process.Message), activityContext);
            return Outcomes("Error");
         }

         await _taskService.RunAsync(task.Process);

         if (task.Process.Status != 200) {
            workflowContext.Fault(new Exception(task.Process.Message), activityContext);
            return Outcomes("Error");
         }

         return Outcomes("Done");
      }

      private static Dictionary<string, string> GetParameters(WorkflowExecutionContext workflowContext) {
         var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
         if (workflowContext.Input != null) {
            foreach (var key in workflowContext.Input.Keys) {
               parameters[key] = workflowContext.Input[key]?.ToString() ?? string.Empty;
            }
         }
         parameters["CorrelationId"] = workflowContext.CorrelationId ?? string.Empty;
         return parameters;
      }
   }
}
