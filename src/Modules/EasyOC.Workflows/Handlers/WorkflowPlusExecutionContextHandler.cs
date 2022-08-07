//using EasyOC.OrchardCore.Excel.Scripting;
using EasyOC.Workflows.Scripting;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System.Threading.Tasks;

namespace EasyOC.Workflows.Handlers
{
    public class WorkflowPlusExecutionContextHandler : WorkflowExecutionContextHandlerBase
    {
        private readonly IWorkflowScriptEvaluator _expressionEvaluator;

        public WorkflowPlusExecutionContextHandler(IWorkflowScriptEvaluator expressionEvaluator)
        {
            _expressionEvaluator = expressionEvaluator;

        }

        public override Task EvaluatingScriptAsync(WorkflowExecutionScriptContext context)
        {
            //context.ScopedMethodProviders.Add(new FreeSqlWorkflowMethodsProvider());
            context.ScopedMethodProviders.Add(new WorkflowPlusMethodsProvider(context.WorkflowContext, _expressionEvaluator));
            context.ScopedMethodProviders.Add(new PowerShellWorkflowMethodsProvider());
            return Task.CompletedTask;
        }
    }
}



