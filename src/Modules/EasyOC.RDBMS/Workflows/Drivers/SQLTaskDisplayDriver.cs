using EasyOC.RDBMS.Workflows.Activities;
using EasyOC.RDBMS.Workflows.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Workflows.Display;
using OrchardCore.Workflows.Models;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.RDBMS.Workflows.Drivers
{
    public class SQLTaskDisplayDriver : ActivityDisplayDriver<SQLTask, SQLTaskViewModel>
    {
        private readonly ISession _session;
        private readonly IStringLocalizer S;

        public SQLTaskDisplayDriver(ISession session, IStringLocalizer<SQLTaskDisplayDriver> s)
        {
            _session = session;
            S = s;
        }

        protected override async ValueTask EditActivityAsync(SQLTask activity, SQLTaskViewModel model)
        {
            model.ExcuteMode = activity.ExcuteMode;
            model.PropertyName = activity.PropertyName;
            model.SQLCommandText = activity.SQLCommandText.Expression;
            model.ConnectionConfigId = activity.ConnectionConfigId;
            model.UseShellDbConnection = activity.UseShellDbConnection;
            var connectionSettings = await _session.Query<ContentItem, ContentItemIndex>()
                                        .Where(x => x.ContentType == "DbConnectionConfig" && (x.Published || x.Latest)).ListAsync();
            model.AllConnections = connectionSettings.Select(x => new SelectListItem() { Text = S[x.DisplayText], Value = x.ContentItemId });
            await Task.CompletedTask;
        }
        protected override void UpdateActivity(SQLTaskViewModel model, SQLTask activity)
        {
            activity.SQLCommandText = new WorkflowExpression<string>(model.SQLCommandText);
            activity.ExcuteMode = model.ExcuteMode;
            activity.PropertyName = model.PropertyName;
            activity.ConnectionConfigId = model.ConnectionConfigId;
            activity.UseShellDbConnection = model.UseShellDbConnection;
        }
    }
}



