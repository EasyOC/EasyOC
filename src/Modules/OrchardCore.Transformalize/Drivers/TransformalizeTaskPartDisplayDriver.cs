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
using TransformalizeModule.Services.Contracts;
using TransformalizeModule.ViewModels;

namespace TransformalizeModule.Drivers {
   public class TransformalizeTaskPartDisplayDriver : ContentPartDisplayDriver<TransformalizeTaskPart> {

      private readonly IStringLocalizer S;
      private readonly IHtmlLocalizer<TransformalizeTaskPartDisplayDriver> H;
      private readonly IConfigurationContainer _container;
      private readonly INotifier _notifier;
      private readonly ISignal _signal;

      public TransformalizeTaskPartDisplayDriver(
         IStringLocalizer<TransformalizeTaskPartDisplayDriver> localizer,
         IHtmlLocalizer<TransformalizeTaskPartDisplayDriver> htmlLocalizer,
         IConfigurationContainer container,
         INotifier notifier,
         ISignal signal
      ) {
         S = localizer;
         H = htmlLocalizer;
         _container = container;
         _notifier = notifier;
         _signal = signal;
      }

      public override IDisplayResult Edit(TransformalizeTaskPart part) {
         return Initialize<EditTransformalizeTaskPartViewModel>("TransformalizeTaskPart_Edit", model => {
            model.TransformalizeTaskPart = part;
            model.Arrangement = part.Arrangement;
         }).Location("Content:1");
      }

      public override async Task<IDisplayResult> UpdateAsync(TransformalizeTaskPart part, IUpdateModel updater, UpdatePartEditorContext context) {

         var model = new EditTransformalizeTaskPartViewModel();

         if (await updater.TryUpdateModelAsync(model, Prefix)) {
            part.Arrangement.Text = model.Arrangement.Text;
         }

         try {
            var logger = new MemoryLogger(LogLevel.Error);
            var process = _container.CreateScope(model.Arrangement.Text, part.ContentItem, new Dictionary<string, string>(), false).Resolve<Process>();
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
