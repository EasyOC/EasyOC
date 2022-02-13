using OrchardCore.ContentManagement;
using OrchardCore.ContentFields.Fields;

namespace TransformalizeModule.Models {
   public class TransformalizeReportPart : ContentPart {
      public TransformalizeReportPart() {

         Arrangement = new TextField() { Text = @"<cfg name=""report"">
   <parameters>
   </parameters>
   <connections>
      <add name=""input"" provider="""" />
   </connections>
   <entities>
      <add name=""entity"">
         <fields>
         </fields>
      </add>
   </entities>
</cfg>" };
         PageSizes = new TextField();
         PageSizesExtended = new TextField();

         BulkActions = new BooleanField();
         BulkActionValueField = new TextField();
         BulkActionCreateTask = new TextField();
         BulkActionWriteTask = new TextField();
         BulkActionSummaryTask = new TextField();
         BulkActionRunTask = new TextField();
         BulkActionSuccessTask = new TextField();
         BulkActionFailTask = new TextField();

         Map = new BooleanField();
         MapDescriptionField = new TextField { Text = "geojson-description" };
         MapLatitudeField = new TextField { Text = "latitude" };
         MapLongitudeField = new TextField { Text = "longitude" };
         MapColorField = new TextField { Text = "geojson-color" };
         MapRadiusField = new TextField { Text = "7" };
         MapOpacityField = new TextField { Text = "0.8" };

         Calendar = new BooleanField();
         CalendarIdField = new TextField { Text = "id" };
         CalendarTitleField = new TextField { Text = "title" };
         CalendarUrlField = new TextField { Text = "url" };
         CalendarClassField = new TextField { Text = "class" };
         CalendarStartField = new TextField { Text = "start" };
         CalendarEndField = new TextField { Text = "end" };

      }
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
      public TextField MapDescriptionField { get; set; }
      public TextField MapLatitudeField { get; set; }
      public TextField MapLongitudeField { get; set; }
      public TextField MapColorField { get; set; }
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
