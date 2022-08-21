using EasyOC.RDBMS.Services;
using EasyOC.RDBMS.Workflows.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.Workflows.Activities
{
    public class ETLFromRDBMS : Activity
    {
        private readonly IStringLocalizer S;
        private readonly IWorkflowScriptEvaluator _scriptEvaluator;
        private readonly IFreeSql freeSql;
        private readonly IRDBMSAppService _rDBMSAppService;

        public ETLFromRDBMS(IStringLocalizer<ETLFromRDBMS> s, IWorkflowScriptEvaluator scriptEvaluator, IFreeSql freeSql, IRDBMSAppService rDBMSAppService)
        {
            S = s;
            _scriptEvaluator = scriptEvaluator;
            this.freeSql = freeSql;
            _rDBMSAppService = rDBMSAppService;
        }


        public override string Name => "ETLFromRDBMS";

        public override LocalizedString DisplayText => S["Extute a ETL task,witch can be load Data from RDBMS."];

        public override LocalizedString Category => S["ETL"];
        public string PropertyName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(S["Done"]);
        }
        public bool UseShellDbConnection
        {
            get; set;
        } = true;

        public string ConnectionConfigId
        {
            get; set;
        }

        /// <summary>
        /// SQLResultType,DataSet,SignleValue or None
        /// </summary>
        public SQLResultType ExcuteMode
        {
            get; set;
        }

        /// <summary>
        /// The script can call any available functions, including setOutcome().
        /// </summary>
        public WorkflowExpression<string> SQLCommandText
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

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
                switch (ExcuteMode)
                {
                    case SQLResultType.DataTable:
                        workflowContext.Properties[PropertyName] = await fsSql.Ado.ExecuteDataTableAsync(sqlText);
                        break;
                    case SQLResultType.DataSet:
                        workflowContext.Properties[PropertyName] = await fsSql.Ado.ExecuteDataSetAsync(sqlText);
                        break;
                    case SQLResultType.SignleValue:
                        workflowContext.Properties[PropertyName] = await fsSql.Ado.ExecuteScalarAsync(sqlText);
                        break;
                    default:
                        workflowContext.Properties[PropertyName] = await fsSql.Ado.ExecuteNonQueryAsync(sqlText);
                        break;
                }
                return Outcomes("Done");
            }
            catch (Exception ex)
            {
                workflowContext.Fault(ex, activityContext);
                return Noop();
            }
        }
    }
}



