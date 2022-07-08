using EasyOC.Workflows.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.Workflows.Activities
{
    public class WorkflowFaultEvent : EventActivity
    {
        private readonly IStringLocalizer<WorkflowFaultEvent> S;
        private readonly IWorkflowScriptEvaluator _scriptEvaluator;

        public WorkflowFaultEvent(IStringLocalizer<WorkflowFaultEvent> stringLocalizer,
             IWorkflowScriptEvaluator scriptEvaluator
            )
        {
            S = stringLocalizer;
            _scriptEvaluator = scriptEvaluator;
        }
        public override string Name => nameof(WorkflowFaultEvent);


        public WorkflowExpression<bool> ErrorFilter
        {
            get => GetProperty(() => new WorkflowExpression<bool>(getDefaultValue()));
            set => SetProperty(value);
        }

        private string getDefaultValue()
        {
            var sample = $@"
//sample
var errorInfo= input('{WorkflowFaultModel.WorkflowFaultInputKey}');
var result=  errorInfo.{nameof(WorkflowFaultModel.WorkflowName)}== 'WorkflowName' ||
errorInfo.{nameof(WorkflowFaultModel.WorkflowId)}== 'WorkflowId' ||
errorInfo.{nameof(WorkflowFaultModel.ErrorMessage)}.indexOf('ErrorStr') ||
errorInfo.{nameof(WorkflowFaultModel.ExceptionDetails)}.indexOf('ErrorStr') ||
errorInfo.{nameof(WorkflowFaultModel.FaultMessage)}.indexOf('ErrorStr') ||
errorInfo.{nameof(WorkflowFaultModel.ActivityDisplayName)}== 'ActivityDisplayName' ||
errorInfo.{nameof(WorkflowFaultModel.ActivityTypeName)}== 'ActivityTypeName' ||
errorInfo.{nameof(WorkflowFaultModel.ActivityId)}== 'ActivityId' ||
errorInfo.{nameof(WorkflowFaultModel.ExcutedActivityCount)}== 20
return result;
            ";
            return sample;
        }
        public string TriggeredWorkflowName { get; set; }
        public override LocalizedString DisplayText => S["Catch Workflow Fault Evnet"];

        public override LocalizedString Category => S["Background"];

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(S["Done"]);
        }
        public override async Task<bool> CanExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            var faultModel = workflowContext.Input[WorkflowFaultModel.WorkflowFaultInputKey] as WorkflowFaultModel;

            //避免自己抓自己，死循环
            if (faultModel == null || faultModel.WorkflowName == workflowContext.WorkflowType.Name)
            {
                return false;
            }
            var result = await _scriptEvaluator.EvaluateAsync(ErrorFilter, workflowContext);
            return result;
        }

        public override Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return base.ExecuteAsync(workflowContext, activityContext);
        }

    }
}



