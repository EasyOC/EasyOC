using EasyOC.ContentExtensions.Models;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace EasyOC.ContentExtensions.Drivers
{
    public class ScriptHandlerDisplayDriver : ContentTypeDefinitionDisplayDriver
    {
        private readonly IStringLocalizer T;

        public ScriptHandlerDisplayDriver(IStringLocalizer<ScriptHandlerDisplayDriver> stringLocalizer)
        {
            T = stringLocalizer;
        }

        public override IDisplayResult Edit(ContentTypeDefinition definition)
        {
            return Initialize<HandlerScripts>(
                "ContentTypeScriptHandler_Edit",
                viewModel => {
                    var model = definition.GetSettings<HandlerScripts>();
                    viewModel.CreateHandlerScript = model.CreateHandlerScript;
                    viewModel.UpdateHandlerScript = model.UpdateHandlerScript;
                    viewModel.DeleteHandlerScript = model.DeleteHandlerScript;
                    viewModel.PublishHandlerScript = model.PublishHandlerScript;
                })
                .Location("Content:7");
        }

        

        public override async Task<IDisplayResult> UpdateAsync(ContentTypeDefinition definition, UpdateTypeEditorContext context)
        {
            var model = new HandlerScripts();

            await context.Updater.TryUpdateModelAsync(model, Prefix);
            context.Builder.WithSettings(model);
            return await base.UpdateAsync(definition, context);
        }
    }
}