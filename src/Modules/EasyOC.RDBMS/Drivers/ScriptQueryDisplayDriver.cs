using EasyOC.RDBMS.Queries.ScriptQuery;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Queries;
using System.Threading.Tasks;
using System;
using EasyOC.RDBMS.ViewModels;

namespace EasyOC.RDBMS.Drivers
{
    public class ScriptQueryDisplayDriver : DisplayDriver<Query, ScriptQuery>
    {
        private readonly IStringLocalizer S;

        public ScriptQueryDisplayDriver(IStringLocalizer<ScriptQueryDisplayDriver> stringLocalizer)
        {
            S = stringLocalizer;
        }

        public override IDisplayResult Display(ScriptQuery query, IUpdateModel updater)
        {
            return Combine(
                Dynamic("ScriptQuery_SummaryAdmin", model =>
                {
                    model.Query = query;
                }).Location("Content:5"),
                Dynamic("ScriptQuery_Buttons_SummaryAdmin", model =>
                {
                    model.Query = query;
                }).Location("Actions:2")
            );
        }

        public override IDisplayResult Edit(ScriptQuery query, IUpdateModel updater)
        {
            return Initialize<ScriptQueryViewModel>("ScriptQuery_Edit", model =>
            {
                model.Script = query.Scripts;
                model.ReturnDocuments = query.ReturnDocuments;

                // Extract query from the query string if we come from the main query editor
                if (string.IsNullOrEmpty(query.Scripts))
                {
                    updater.TryUpdateModelAsync(model, "", m => m.Script);
                }
            }).Location("Content:6");
        }

        public override async Task<IDisplayResult> UpdateAsync(ScriptQuery model, IUpdateModel updater)
        {
            var viewModel = new ScriptQueryViewModel();
            if (await updater.TryUpdateModelAsync(viewModel, Prefix, m => m.Script, m => m.ReturnDocuments))
            {
                model.Scripts = viewModel.Script;
                model.ReturnDocuments = viewModel.ReturnDocuments;
            }

            if (String.IsNullOrWhiteSpace(model.Scripts))
            {
                updater.ModelState.AddModelError(nameof(model.Scripts), S["The query field is required"]);
            }

            return Edit(model, updater);
        }
    }
}
