using EasyOC.OrchardCore.Excel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.Excel.Workflow
{
    public class UploadExcelTask : TaskActivity
    {
        private readonly IStringLocalizer S;
        private readonly IWorkflowScriptEvaluator _expressionEvaluator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExcelAppService _excelAppService;

        public UploadExcelTask(IWorkflowScriptEvaluator scriptEvaluator, 
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<UploadExcelTask> s, IExcelAppService excelAppService)
        {
            _expressionEvaluator = scriptEvaluator;
           _httpContextAccessor = httpContextAccessor;
            S = s;
            _excelAppService = excelAppService;
        }

        public override string Name => nameof(ExcelTask);

        public override LocalizedString DisplayText => S["Load Excel Data From HttpRequest"];

        public override LocalizedString Category => S["Primitives"];


        public WorkflowExpression<string> RowFilter
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }
        //public string ExcelConfigId
        //{
        //    get => GetProperty<string>();
        //    set => SetProperty(value);
        //}

        public string TargetFieldName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        public string PropertyName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public WorkflowExpression<object> ExtraScripts
        {
            get => GetProperty(() => new WorkflowExpression<object>());
            set => SetProperty(value);
        }


        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(S["Done"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            try
            {
                var rowFilter = await _expressionEvaluator.EvaluateAsync(RowFilter, workflowContext);
                var tableData = await _excelAppService.GetExcelDataFromConfigAsync(FilePath, rowFilter);

                if (!string.IsNullOrEmpty(ExtraScripts.Expression))
                {
                    var injectResult = new WorkflowExpression<object>(

                        $@"
                           var {PropertyName}={Newtonsoft.Json.JsonConvert.SerializeObject(tableData)};
                            " + ExtraScripts.Expression
                        );

                    var result = await _expressionEvaluator.EvaluateAsync(injectResult, workflowContext);
                    workflowContext.Properties[PropertyName] = result;
                }
                else
                {
                    workflowContext.Properties[PropertyName] = tableData;
                }
                return Outcomes("Done");
            }
            catch (Exception ex)
            {
                workflowContext.Fault(ex, activityContext);
                throw;
            };
        }
    }
}



