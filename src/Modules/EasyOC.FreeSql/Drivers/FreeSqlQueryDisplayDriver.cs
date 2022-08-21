using EasyOC.FreeSql.Queries;
using EasyOC.FreeSql.ViewModels;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Queries;
using System;
using System.Threading.Tasks;

namespace EasyOC.FreeSql.Drivers
{
    public class FreeSqlQueryDisplayDriver : DisplayDriver<Query, FreeSqlQuery>
    {
        private readonly IStringLocalizer S;

        public FreeSqlQueryDisplayDriver(IStringLocalizer<FreeSqlQueryDisplayDriver> stringLocalizer)
        {
            S = stringLocalizer;
        }

        public override IDisplayResult Display(FreeSqlQuery query, IUpdateModel updater)
        {
            return Combine(
                Dynamic("FreeSqlQuery_SummaryAdmin", model =>
                {
                    model.Query = query;
                }).Location("Content:5"),
                Dynamic("FreeSqlQuery_Buttons_SummaryAdmin", model =>
                {
                    model.Query = query;
                }).Location("Actions:2")
            );
        }

        public override IDisplayResult Edit(FreeSqlQuery query, IUpdateModel updater)
        {
            return Initialize<FreeSqlQueryViewModel>("FreeSqlQuery_Edit", model =>
            {
                model.Query = query.Template;
                model.ReturnDocuments = query.ReturnDocuments;

                // Extract query from the query string if we come from the main query editor
                if (string.IsNullOrEmpty(query.Template))
                {
                    updater.TryUpdateModelAsync(model, "", m => m.Query);
                }
            }).Location("Content:6");
        }

        public override async Task<IDisplayResult> UpdateAsync(FreeSqlQuery model, IUpdateModel updater)
        {
            var viewModel = new FreeSqlQueryViewModel();
            if (await updater.TryUpdateModelAsync(viewModel, Prefix, m => m.Query, m => m.ReturnDocuments))
            {
                model.Template = viewModel.Query;
                model.ReturnDocuments = viewModel.ReturnDocuments;
            }

            if (String.IsNullOrWhiteSpace(model.Template))
            {
                updater.ModelState.AddModelError(nameof(model.Template), S["The query field is required"]);
            }

            return Edit(model, updater);
        }
    }
}
