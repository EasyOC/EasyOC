using OrchardCore.Scripting;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.WorkflowPlus.Scripting
{
    public class WorkflowPlusMethodsProvider : IGlobalMethodProvider
    {
        private readonly WorkflowExecutionContext workflowContext;
        private readonly IWorkflowScriptEvaluator _expressionEvaluator;

        public WorkflowPlusMethodsProvider(WorkflowExecutionContext workflowContext, IWorkflowScriptEvaluator expressionEvaluator)
        {
            this.workflowContext = workflowContext;
            _expressionEvaluator = expressionEvaluator;
        }

        public IEnumerable<GlobalMethod> GetMethods()
        {
            return new[] {
               new GlobalMethod
                {
                    Name = "setCorrelationId",
                    Method = serviceProvider => (Action<string>)((correlationId) => workflowContext.CorrelationId = correlationId)
                },
               //new GlobalMethod
               // {
               //     Name = "log",
               //     Method = serviceProvider => (Action<string>)((script) =>
               //     {
               //         var result= _expressionEvaluator.EvaluateAsync(new WorkflowExpression<string>(script),workflowContext);
               //         _logger.LogDebug(result.Result);
               //     })
               // }
            };
        }
    }
}



