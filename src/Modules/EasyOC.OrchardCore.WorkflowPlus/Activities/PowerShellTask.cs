using EasyOC.OrchardCore.WorkflowPlus.Scripting.Powershell;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.WorkflowPlus.Activities
{
    public class PowerShellTask : Activity
    {
        private readonly IStringLocalizer S;
        private readonly IWorkflowScriptEvaluator _scriptEvaluator;
        private readonly ILogger<PowerShellTask> logger;

        public PowerShellTask(IStringLocalizer<PowerShellTask> s, IWorkflowScriptEvaluator scriptEvaluator, ILogger<PowerShellTask> logger)
        {
            S = s;
            _scriptEvaluator = scriptEvaluator;
            this.logger = logger;
        }
        /// <summary>
        /// The script can call any available functions, including setOutcome().
        /// </summary>
        public WorkflowExpression<string> ScriptText
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public override string Name => nameof(PowerShellTask);
        public IList<string> AvailableOutcomes
        {
            get => GetProperty(() => new List<string> { "Done" });
            set => SetProperty(value);
        }

        public bool UseJavascript
        {

            get => GetProperty<bool>();
            set => SetProperty(value);
        }
        public string PropertyName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        public override LocalizedString DisplayText => S["Run Powershell Script Task"];



        public override LocalizedString Category => S[AppConsts.WorkflowDefaultCategory];

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            try
            {
                var commandText = string.Empty;
                if (UseJavascript)
                {
                    commandText = await _scriptEvaluator.EvaluateAsync(ScriptText, workflowContext);
                }
                else
                {
                    commandText = ScriptText.Expression;
                }
                var result = PowerShellExecuter.Excute(commandText);
                if (!string.IsNullOrEmpty(PropertyName))
                {
                    workflowContext.Properties[PropertyName] = result;
                }
                return Outcomes("Done");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                workflowContext.Fault(ex, activityContext);
                throw new Exception("Powershell excution error :" + ex.Message);
            }
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(S["Done"]);
        }
    }
}



