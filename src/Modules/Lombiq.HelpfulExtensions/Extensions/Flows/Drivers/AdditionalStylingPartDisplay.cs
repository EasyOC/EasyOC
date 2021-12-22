using Lombiq.HelpfulExtensions.Extensions.Flows.Models;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace Lombiq.HelpfulExtensions.Extensions.Flows.Drivers
{
    public class AdditionalStylingPartDisplay : ContentDisplayDriver
    {
        public override IDisplayResult Edit(ContentItem model, IUpdateModel updater) =>
            Initialize<AdditionalStylingPart>(
                $"{nameof(AdditionalStylingPart)}_Edit",
                viewModel => PopulateViewModel(model, viewModel))
            .Location("Footer:3");

        public override async Task<IDisplayResult> UpdateAsync(ContentItem model, IUpdateModel updater)
        {
            var additionalStylingPart = model.As<AdditionalStylingPart>();

            if (additionalStylingPart == null)
            {
                return null;
            }

            await model.AlterAsync<AdditionalStylingPart>(model => updater.TryUpdateModelAsync(model, Prefix));

            return await EditAsync(model, updater);
        }

        private static void PopulateViewModel(ContentItem model, AdditionalStylingPart viewModel)
        {
            var additionalStylingPart = model.As<AdditionalStylingPart>();

            if (additionalStylingPart != null)
            {
                viewModel.CustomClasses = additionalStylingPart.CustomClasses;
                viewModel.RemoveGridExtensionClasses = additionalStylingPart.RemoveGridExtensionClasses;
            }
        }
    }
}



