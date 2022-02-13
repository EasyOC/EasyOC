using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Cache;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Logging;
using TransformalizeModule.Models;
using TransformalizeModule.Services;
using TransformalizeModule.Services.Contracts;
using TransformalizeModule.ViewModels;
using IContainer = TransformalizeModule.Services.Contracts.IContainer;

namespace TransformalizeModule.Drivers {
   public class TransformalizeFormPartDisplayDriver : ContentPartDisplayDriver<TransformalizeFormPart> {

      private readonly IStringLocalizer S;
      private readonly IHtmlLocalizer<TransformalizeFormPartDisplayDriver> H;
      private readonly IConfigurationContainer _configurationContainer;
      private readonly IContainer _container;
      private readonly CombinedLogger<TransformalizeFormPartDisplayDriver> _logger;
      private readonly INotifier _notifier;
      private readonly ISignal _signal;
      private readonly ISettingsService _settings;

      public TransformalizeFormPartDisplayDriver(
         IStringLocalizer<TransformalizeFormPartDisplayDriver> localizer,
         IHtmlLocalizer<TransformalizeFormPartDisplayDriver> htmlLocalizer,
         CombinedLogger<TransformalizeFormPartDisplayDriver> logger,
         IConfigurationContainer configurationContainer,
         IContainer container,
         INotifier notifier,
         ISignal signal,
         ISettingsService settings
      ) {
         S = localizer;
         H = htmlLocalizer;
         _container = container;
         _logger = logger;
         _configurationContainer = configurationContainer;
         _notifier = notifier;
         _signal = signal;
         _settings = settings;
      }

      public override IDisplayResult Edit(TransformalizeFormPart part) {

         var commands = new AdoFormCommands();
         var process = _configurationContainer.CreateScope(part.Arrangement.Text, part.ContentItem, new Dictionary<string, string>(), false).Resolve<Process>();

         _settings.ApplyCommonSettings(process);

         using (var scope = _container.CreateScope(process, _logger, null)) {
            if (scope.IsRegistered<AdoFormCommandWriter>()) {
               commands = scope.Resolve<AdoFormCommandWriter>().Write();
            }
         }

         return Initialize<EditTransformalizeFormPartViewModel>("TransformalizeFormPart_Edit", model => {
            model.TransformalizeFormPart = part;
            model.Arrangement = part.Arrangement;
            model.LocationEnableHighAccuracy = part.LocationEnableHighAccuracy;
            model.LocationMaximumAge = part.LocationMaximumAge;
            model.LocationTimeout = part.LocationTimeout;
            model.CreateCommand = commands.Create;
            model.InsertCommand = commands.Insert;
            model.UpdateCommand = commands.Update;
            model.SelectCommand = commands.Select;
         }).Location("Content:1");
      }

      public override async Task<IDisplayResult> UpdateAsync(TransformalizeFormPart part, IUpdateModel updater, UpdatePartEditorContext context) {

         var model = new EditTransformalizeFormPartViewModel();

         if (await updater.TryUpdateModelAsync(model, Prefix)) {
            part.Arrangement.Text = model.Arrangement.Text;
            part.LocationEnableHighAccuracy.Value = model.LocationEnableHighAccuracy.Value;
            part.LocationMaximumAge.Value = model.LocationMaximumAge.Value;
            part.LocationTimeout.Value = model.LocationTimeout.Value;
         }

         try {
            var logger = new MemoryLogger(LogLevel.Error);
            var process = _configurationContainer.CreateScope(model.Arrangement.Text, part.ContentItem, new Dictionary<string, string>(), false).Resolve<Process>();
            if (process.Errors().Any()) {
               foreach (var error in process.Errors()) {
                  updater.ModelState.AddModelError(Prefix, S[error]);
               }
            }

            if (process.Warnings().Any()) {
               foreach (var warning in process.Warnings()) {
                  _notifier.Warning(H[warning]);
               }
            }

            if (!process.Connections.Any(c => c.Table != "[default]" && !string.IsNullOrEmpty(c.Table))) {
               updater.ModelState.AddModelError(Prefix, S["A form requires one connection to have a table defined.  The submissions are stored in this table."]);
            }

            if (process.Parameters.Where(p => p.PrimaryKey).Count() != 1) {
               updater.ModelState.AddModelError(Prefix, S["A form requires one parameter to be marked as the primary key."]);
            }

         } catch (Exception ex) {
            updater.ModelState.AddModelError(Prefix, S[ex.Message]);
         }

         if (updater.ModelState.IsValid) {
            await _signal.SignalTokenAsync(Common.GetCacheKey(part.ContentItem.Id));
         }

         return Edit(part, context);

      }

   }
}
