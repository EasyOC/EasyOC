using EasyOC.OrchardCore.WorkflowPlus.Scripting.Powershell;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Users.Models;
using OrchardCore.Users.Services;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.WorkflowPlus.Activities
{
    public class CreateUserTask : Activity
    {
        private readonly IStringLocalizer S;
        private readonly IWorkflowScriptEvaluator _scriptEvaluator;
        private readonly ILogger logger;
        private readonly IUserService _userService;
        private readonly INotifier _notifier;
        private readonly IHtmlLocalizer H;

        public CreateUserTask(IStringLocalizer<CreateUserTask> s,
            IWorkflowScriptEvaluator scriptEvaluator,
            ILogger<CreateUserTask> logger, INotifier notifier, IUserService userService
            , IHtmlLocalizer<CreateUserTask> htmlLocalizer)
        {
            S = s;
            _scriptEvaluator = scriptEvaluator;
            this.logger = logger;
            _notifier = notifier;
            _userService = userService;
            H = htmlLocalizer;
        }
        /// <summary>
        /// The script can call any available functions, including setOutcome().
        /// </summary>
        public WorkflowExpression<string> Script
        {
            get => GetProperty(() => new WorkflowExpression<string>(
                $@" return JSON.stringify({JsonConvert.SerializeObject(new User(), Formatting.Indented)});"));
            set => SetProperty(value);
        }

        public override string Name => nameof(CreateUserTask);


        public override LocalizedString DisplayText => S["Create User Task"];



        public override LocalizedString Category => S["User"];

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            try
            {
                var userContnet = await _scriptEvaluator.EvaluateAsync(Script, workflowContext);

                var errorList = new List<string>();
                var user = await _userService.CreateUserAsync(JsonConvert.DeserializeObject<User>(userContnet), null, (key, value) =>
              {
                  errorList.Add(H[$"{key}:{value}"].Value);
              });
                if (errorList.Count > 0)
                {
                    workflowContext.LastResult = errorList;
                    return Outcomes("Faild");
                }
                workflowContext.LastResult = user;
                return Outcomes("Done");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                workflowContext.Fault(ex, activityContext);
                return Outcomes("Faild");
            }
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(S["Done"], S["Faild"]);
        }
    }
}



