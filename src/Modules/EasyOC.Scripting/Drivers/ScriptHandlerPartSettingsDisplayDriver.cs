using EasyOC.Scripting.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using StatCan.OrchardCore.ContentFields.MultiValueTextField.ViewModels;
using System.Threading.Tasks;

namespace OrchardCore.Title.Settings
{
    public class ScriptHandlerPartSettingsDisplayDriver : ContentTypePartDefinitionDisplayDriver<ScriptHandlerPart>
    {
        private readonly IStringLocalizer S;

        public ScriptHandlerPartSettingsDisplayDriver(IStringLocalizer<ScriptHandlerPartSettingsDisplayDriver> localizer)
        {
            S = localizer;
        }

        public override IDisplayResult Edit(ContentTypePartDefinition contentTypePartDefinition, IUpdateModel updater)
        {
            return Initialize<ScriptHandlerPartSettingsViewModel>("ScriptHandlerPartSettings_Edit", model =>
            {
                var settings = contentTypePartDefinition.GetSettings<ScriptHandlerPartSettings>();
                model.Disabled = settings.Disabled;
                model.ScriptHandlerSettingItems = settings.ScriptHandlerSettingItems;
            }).Location("Content");
        }

        public override async Task<IDisplayResult> UpdateAsync(ContentTypePartDefinition contentTypePartDefinition, UpdateTypePartEditorContext context)
        {
            var model = new ScriptHandlerPartSettingsViewModel();

            await context.Updater.TryUpdateModelAsync(model, Prefix,
                m => m.Disabled,
                m => m.ScriptHandlerSettingItems);

            context.Builder.WithSettings(new ScriptHandlerPartSettings { Disabled = model.Disabled, ScriptHandlerSettingItems = model.ScriptHandlerSettingItems });

            return Edit(contentTypePartDefinition, context.Updater);
        }
    }
}
