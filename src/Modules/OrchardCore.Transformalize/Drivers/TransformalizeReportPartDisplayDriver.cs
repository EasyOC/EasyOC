using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using TransformalizeModule.Models;
using TransformalizeModule.ViewModels;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.DisplayManagement.Notify;
using System;
using Transformalize.Logging;
using Transformalize.Contracts;
using System.Collections.Generic;
using TransformalizeModule.Services.Contracts;
using OrchardCore.Environment.Cache;
using Autofac;
using Transformalize.Configuration;
using System.Linq;
using OrchardCore.ContentFields.Fields;
using GraphQL;

namespace TransformalizeModule.Drivers {
   public class TransformalizeReportPartDisplayDriver : ContentPartDisplayDriver<TransformalizeReportPart> {

      private readonly IStringLocalizer<TransformalizeReportPartDisplayDriver> S;
      private readonly IHtmlLocalizer<TransformalizeReportPartDisplayDriver> H;
      private readonly IConfigurationContainer _container;
      private readonly INotifier _notifier;
      private readonly ISignal _signal;

      public TransformalizeReportPartDisplayDriver(
         IStringLocalizer<TransformalizeReportPartDisplayDriver> localizer,
         IHtmlLocalizer<TransformalizeReportPartDisplayDriver> htmlLocalizer,
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

      // change display for admin content items view?
      // public override IDisplayResult Display(TransformalizeReportPart part) => View(nameof(TransformalizeReportPart), part);

      public override IDisplayResult Edit(TransformalizeReportPart part) {
         return Initialize<EditTransformalizeReportPartViewModel>("TransformalizeReportPart_Edit", model => {
            // it did not work out when i tried to flatten model (e.g. BulkActions.Value => BulkActions (a bool))
            model.TransformalizeReportPart = part;
            model.Arrangement = part.Arrangement;
            model.PageSizes = part.PageSizes;
            model.PageSizesExtended = part.PageSizesExtended;

            model.BulkActions = part.BulkActions;
            model.BulkActionValueField = part.BulkActionValueField;
            model.BulkActionCreateTask = part.BulkActionCreateTask;
            model.BulkActionWriteTask = part.BulkActionWriteTask;
            model.BulkActionSummaryTask = part.BulkActionSummaryTask;
            model.BulkActionRunTask = part.BulkActionRunTask;
            model.BulkActionSuccessTask = part.BulkActionSuccessTask;
            model.BulkActionFailTask = part.BulkActionFailTask;

            model.Map = part.Map;
            model.MapColorField = part.MapColorField;
            model.MapDescriptionField = part.MapDescriptionField;
            model.MapLatitudeField = part.MapLatitudeField;
            model.MapLongitudeField = part.MapLongitudeField;
            model.MapRadiusField = part.MapRadiusField;
            model.MapOpacityField = part.MapOpacityField;

            model.Calendar = part.Calendar;
            model.CalendarIdField = part.CalendarIdField;
            model.CalendarTitleField = part.CalendarTitleField;
            model.CalendarUrlField = part.CalendarUrlField;
            model.CalendarClassField = part.CalendarClassField;
            model.CalendarStartField = part.CalendarStartField;
            model.CalendarEndField = part.CalendarEndField;

         }).Location("Content:1");
      }

      public override async Task<IDisplayResult> UpdateAsync(TransformalizeReportPart part, IUpdateModel updater, UpdatePartEditorContext context) {

         // this driver override makes sure all the 
         // part fields are updated before the arrangement model is updated / validated

         //_notifier.Information(H["Part - Bulk Actions:{0}", part.BulkActions.Value]);
         //_notifier.Information(H["Part - Bulk Action Field:{0}", part.BulkActionValueField.Text]);

         var model = new EditTransformalizeReportPartViewModel();

         if (await updater.TryUpdateModelAsync(model, Prefix)) {
            //_notifier.Information(H["Model - Bulk Actions:{0}", model.BulkActions.Value]);
            //_notifier.Information(H["Model - Bulk Action Field:{0}", model.BulkActionValueField.Text]);

            part.Arrangement.Text = model.Arrangement.Text;
            part.PageSizes.Text = model.PageSizes.Text;
            part.PageSizesExtended.Text = model.PageSizes.Text;

            part.BulkActions.Value = model.BulkActions.Value;
            part.BulkActionValueField.Text = model.BulkActionValueField.Text;
            part.BulkActionCreateTask.Text = model.BulkActionCreateTask.Text;
            part.BulkActionWriteTask.Text = model.BulkActionWriteTask.Text;
            part.BulkActionSummaryTask.Text = model.BulkActionSummaryTask.Text;
            part.BulkActionRunTask.Text = model.BulkActionRunTask.Text;
            part.BulkActionSuccessTask.Text = model.BulkActionSuccessTask.Text;
            part.BulkActionFailTask.Text = model.BulkActionFailTask.Text;

            part.Map.Value = model.Map.Value;
            part.MapColorField.Text = model.MapColorField.Text;
            part.MapDescriptionField.Text = model.MapDescriptionField.Text;
            part.MapLatitudeField.Text = model.MapLatitudeField.Text;
            part.MapLongitudeField.Text = model.MapLongitudeField.Text;
            part.MapRadiusField.Text = model.MapRadiusField.Text;
            part.MapOpacityField.Text = model.MapOpacityField.Text;

            part.Calendar.Value = model.Calendar.Value;
            part.CalendarClassField.Text = model.CalendarClassField.Text;
            part.CalendarIdField.Text = model.CalendarIdField.Text;
            part.CalendarTitleField.Text = model.CalendarTitleField.Text;
            part.CalendarUrlField.Text = model.CalendarUrlField.Text;
            part.CalendarStartField.Text = model.CalendarStartField.Text;
            part.CalendarEndField.Text = model.CalendarEndField.Text;

         }

         if (model.BulkActions.Value) {
            if (string.IsNullOrEmpty(model.BulkActionValueField.Text)) {
               updater.ModelState.AddModelError(Prefix, S["Please set the bulk action value field for bulk actions."]);
            }
         }

         CheckPageSizes(model.PageSizes, updater);
         CheckPageSizes(model.PageSizesExtended, updater);

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
            if (process.Entities.Any()) {

               var fields = process.Entities[0].GetAllFields().ToArray();

               if (part.BulkActions.Value) {
                  if (!string.IsNullOrEmpty(part.BulkActionValueField.Text)) {
                     if (fields.All(f => f.Alias != part.BulkActionValueField.Text)) {
                        updater.ModelState.AddModelError(Prefix, S["The field {0} does not exist.", part.BulkActionValueField.Text]);
                     }
                  }
               }

               if (part.Calendar.Value) {
                  if (fields.All(f => f.Alias != part.CalendarIdField.Text)) {
                     updater.ModelState.AddModelError(Prefix, S["The field {0} used for calendar id does not exist.", part.CalendarIdField.Text]);
                  }
                  if (fields.All(f => f.Alias != part.CalendarUrlField.Text)) {
                     updater.ModelState.AddModelError(Prefix, S["The field {0} used for calendar URL does not exist.", part.CalendarUrlField.Text]);
                  }
                  if (fields.All(f => f.Alias != part.CalendarTitleField.Text)) {
                     updater.ModelState.AddModelError(Prefix, S["The field {0} used for calendar title does not exist.", part.CalendarTitleField.Text]);
                  }
                  if (fields.All(f => f.Alias != part.CalendarClassField.Text)) {
                     updater.ModelState.AddModelError(Prefix, S["The field {0} used for calendar class does not exist.", part.CalendarClassField.Text]);
                  }
                  if (fields.All(f => f.Alias != part.CalendarStartField.Text)) {
                     updater.ModelState.AddModelError(Prefix, S["The field {0} used for calendar start does not exist.", part.CalendarStartField.Text]);
                  }
                  if (fields.All(f => f.Alias != part.CalendarEndField.Text)) {
                     updater.ModelState.AddModelError(Prefix, S["The field {0} used for calendar end does not exist.", part.CalendarEndField.Text]);
                  }
               }

               if (part.Map.Value) {

                  if (fields.All(f => f.Alias != part.MapDescriptionField.Text)) {
                     updater.ModelState.AddModelError(Prefix, S["The field {0} used for map description does not exist.", part.MapDescriptionField.Text]);
                  }
                  if (fields.All(f => f.Alias != part.MapLatitudeField.Text)) {
                     updater.ModelState.AddModelError(Prefix, S["The field {0} used for map latitude does not exist.", part.MapLatitudeField.Text]);
                  }
                  if (fields.All(f => f.Alias != part.MapLongitudeField.Text)) {
                     updater.ModelState.AddModelError(Prefix, S["The field {0} used for map longitude does not exist.", part.MapLongitudeField.Text]);
                  }

                  // Map Color #ffc0cb
                  if (string.IsNullOrWhiteSpace(part.MapColorField.Text)) {
                     part.MapColorField.Text = "#ffc0cb";
                  } else {
                     if (fields.All(f => f.Alias != part.MapColorField.Text)) {
                        updater.ModelState.AddModelError(Prefix, S["The field {0} used for map color does not exist.", part.MapColorField.Text]);
                     }
                  }

                  // Map Opacity
                  if (string.IsNullOrWhiteSpace(part.MapOpacityField.Text)) {
                     part.MapOpacityField.Text = "0.8";
                  } else {
                     if (double.TryParse(part.MapOpacityField.Text, out double opacity)) {
                        if (opacity < 0 || opacity > 1) {
                           updater.ModelState.AddModelError(Prefix, S["Map opacity must be between 0 and 1."]);
                        }
                     } else {
                        if (fields.All(f => f.Alias != part.MapOpacityField.Text)) {
                           updater.ModelState.AddModelError(Prefix, S["The field {0} used for map opacity does not exist.", part.MapOpacityField.Text]);
                        }
                     }
                  }

                  // Map Radius
                  if (string.IsNullOrWhiteSpace(part.MapRadiusField.Text)) {
                     part.MapRadiusField.Text = "7";
                  } else {
                     if (!int.TryParse(part.MapRadiusField.Text, out int radius)) {
                        if (fields.All(f => f.Alias != part.MapRadiusField.Text)) {
                           updater.ModelState.AddModelError(Prefix, S["The field {0} used for map radius does not exist.", part.MapRadiusField.Text]);
                        }
                     }
                  }
               }

            } else {
               updater.ModelState.AddModelError(Prefix, S["Please define an entity."]);
            }

         } catch (Exception ex) {
            updater.ModelState.AddModelError(Prefix, S[ex.Message]);
         }

         if (updater.ModelState.IsValid) {
            await _signal.SignalTokenAsync(Common.GetCacheKey(part.ContentItem.Id));
         }

         return Edit(part, context);

      }

      public void CheckPageSizes(TextField field, IUpdateModel updater) {
         if (string.IsNullOrWhiteSpace(field.Text)) {
            field.Text = string.Empty;
         } else {
            foreach (var size in field.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
               if (!int.TryParse(size, out _)) {
                  updater.ModelState.AddModelError(Prefix, S["{0} is not a valid integer in page sizes.", size]);
               }
            }
         }
      }


   }
}
