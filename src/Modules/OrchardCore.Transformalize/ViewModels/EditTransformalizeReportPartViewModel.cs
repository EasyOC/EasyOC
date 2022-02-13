using OrchardCore.ContentFields.Fields;
using TransformalizeModule.Models;

namespace TransformalizeModule.ViewModels {
   public class EditTransformalizeReportPartViewModel {
      public TransformalizeReportPart TransformalizeReportPart { get; set; }
      public TextField Arrangement { get; set; }
      public TextField PageSizes { get; set; }
      public TextField PageSizesExtended { get; set; }
      
      public BooleanField BulkActions { get; set; }
      public TextField BulkActionValueField { get; set; }
      public TextField BulkActionCreateTask { get; set; }
      public TextField BulkActionWriteTask { get; set; }
      public TextField BulkActionSummaryTask { get; set; }
      public TextField BulkActionRunTask { get; set; }
      public TextField BulkActionSuccessTask { get; set; }
      public TextField BulkActionFailTask { get; set; }

      public BooleanField Map { get; set; }
      public TextField MapColorField { get; set; }
      public TextField MapDescriptionField { get; set; }
      public TextField MapLatitudeField { get; set; }
      public TextField MapLongitudeField { get; set; }
      public TextField MapRadiusField { get; set; }
      public TextField MapOpacityField { get; set; }

      public BooleanField Calendar { get; set; }
      public TextField CalendarIdField { get; set; }
      public TextField CalendarTitleField { get; set; }
      public TextField CalendarUrlField { get; set; }
      public TextField CalendarClassField { get; set; }
      public TextField CalendarStartField { get; set; }
      public TextField CalendarEndField { get; set; }
   }
}