using EasyOC.OrchardCore.RDBMS.Services;
using EasyOC.OrchardCore.RDBMS.Workflows.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.RDBMS.Workflows.Activities
{
    public class SQLTask : Activity
    {
        private readonly IStringLocalizer S;
        private readonly IWorkflowScriptEvaluator _scriptEvaluator;
        private readonly IFreeSql freeSql;
        private readonly IRDBMSAppService _rDBMSAppService;

        public SQLTask(IStringLocalizer<SQLTask> s, IWorkflowScriptEvaluator scriptEvaluator, IFreeSql freeSql, IRDBMSAppService rDBMSAppService)
        {
            S = s;
            _scriptEvaluator = scriptEvaluator;
            this.freeSql = freeSql;
            _rDBMSAppService = rDBMSAppService;
        }
        /// <summary>
        /// The script can call any available functions, including setOutcome().
        /// </summary>
        public WorkflowExpression<string> SQLCommandText
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public bool UseShellDbConnection
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public string ConnectionConfigId
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        /// <summary>
        /// SQLResultType,DataSet,SignleValue or None
        /// </summary>
        public SQLResultType ExcuteMode
        {
            get => GetProperty<SQLResultType>();
            set => SetProperty(value);
        }

        public override string Name => nameof(SQLTask);
        public IList<string> AvailableOutcomes
        {
            get => GetProperty(() => new List<string> { "Done" });
            set => SetProperty(value);
        }


        public string PropertyName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        public override LocalizedString DisplayText => S["SQL Task"];

        public override LocalizedString Category => S["Primitives"];

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            try
            {
                IFreeSql fsSql;
                if (!UseShellDbConnection)
                {
                    fsSql = await _rDBMSAppService.GetFreeSqlAsync(ConnectionConfigId);
                }
                else
                {
                    fsSql = freeSql;
                }

                var sqlText = await _scriptEvaluator.EvaluateAsync(SQLCommandText, workflowContext);
                object result = null;
                switch (ExcuteMode)
                {
                    case SQLResultType.DataTable:
                        result = await fsSql.Ado.ExecuteDataTableAsync(sqlText);
                        break;
                    case SQLResultType.DataSet:
                        result = await fsSql.Ado.ExecuteDataSetAsync(sqlText);
                        break;
                }
                var tempExpression = new WorkflowExpression<object>($"return {JsonConvert.SerializeObject(result)}");
                var jObject = await _scriptEvaluator.EvaluateAsync(tempExpression, workflowContext);
                workflowContext.Properties[PropertyName] = jObject;
                return Outcomes("Done");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(S["Done"]);
        }
    }
}



