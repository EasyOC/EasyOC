using EasyOC.OrchardCore.Excel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.Excel.Workflow
{
    public class ExcelTask : TaskActivity
    {
        private readonly IStringLocalizer S;
        private readonly IWorkflowScriptEvaluator _expressionEvaluator;
        private readonly IExcelAppService _excelAppService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExcelTask(IWorkflowScriptEvaluator scriptEvaluator, IStringLocalizer<ExcelTask> s, IExcelAppService excelAppService, IHttpContextAccessor httpContextAccessor)
        {
            _expressionEvaluator = scriptEvaluator;
            S = s;
            _excelAppService = excelAppService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override string Name => nameof(ExcelTask);

        public override LocalizedString DisplayText => S["Load Data From Excel"];

        public override LocalizedString Category => S["Primitives"];
        public bool FromUpload
        {
            get => GetProperty(() => false);
            set => SetProperty(value);
        }

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

        public string FilePath
        {
            get => GetProperty(() => string.Empty);
            set => SetProperty(value ?? "");
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
                DataTable tableData = null;
                if (!FromUpload)
                {
                    tableData = await _excelAppService.GetExcelDataFromConfigAsync(FilePath, rowFilter);
                }
                else
                {
                    var postFiles = _httpContextAccessor.HttpContext.Request.Form.Files;
                    if (postFiles.Count > 0)
                    {
                        using (var stream = postFiles[0].OpenReadStream())
                        {
                            tableData = _excelAppService.GetExcelDataFromConfigFromStream(stream, rowFilter);
                        }
                    }

                }

                if (tableData != null && !string.IsNullOrEmpty(ExtraScripts.Expression))
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



