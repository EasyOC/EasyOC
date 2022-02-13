using OrchardCore.Alias.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.Features.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TransformalizeModule {
   public class Migrations : DataMigration {

      private readonly IContentDefinitionManager _contentDefinitionManager;
      private readonly IModuleService _moduleService;
      private readonly ILogger<Migrations> _logger;

      public Migrations(
         IContentDefinitionManager contentDefinitionManager,
         IModuleService moduleService,
         ILogger<Migrations> logger
      ) {
         _contentDefinitionManager = contentDefinitionManager;
         _moduleService = moduleService;
         _logger = logger;
      }

      public int Create() {

         _contentDefinitionManager.AlterPartDefinition("TransformalizeReportPart", part => part
             .WithDisplayName("Transformalize Report Part")
             .WithDescription("Fields for Transformalize Report content type")
             .WithField("Arrangement", field => field
                 .OfType(nameof(TextField))
                 .WithDisplayName("Arrangement")
                 .WithPosition("3")
                 .WithEditor("TransformalizeArrangement")
                 .WithSettings(new TextFieldSettings {
                    Hint = string.Empty,
                    Required = true
                 }
                 )
             ).WithField("PageSizes", field => field
                 .OfType(nameof(TextField))
                 .WithDisplayName("Page Sizes")
                 .WithPosition("4")
                 .WithSettings(new TextFieldSettings {
                    Required = false,
                    Hint = "To overide default page sizes, specify a comma delimited list of page sizes (integers). To use the common page sizes defined in settings, leave it blank.  To disable pagination altogether, set this to 0."
                 }
                )
             ).WithField("BulkActions", field => field
                .OfType(nameof(BooleanField))
                .WithDisplayName("Bulk Actions")
                .WithPosition("10")
                .WithSettings(new BooleanFieldSettings {
                   DefaultValue = false,
                   Hint = "Allow user to select one, many, or all records for a bulk action?",
                   Label = "Bulk Actions"
                }
                )
              ).WithField("BulkActionValueField", field => field
                .OfType(nameof(TextField))
                .WithDisplayName("Bulk Action Value Field")
                .WithPosition("11")
                .WithSettings(new TextFieldSettings {
                   Required = false,
                   Hint = "Specify which field or calculated field provides the value for bulk actions."
                }
                )
              ).WithField("BulkActionCreateTask", field => field
                  .OfType(nameof(TextField))
                  .WithDisplayName("Bulk Action Create Task")
                  .WithPosition("12")
                  .WithSettings(new TextFieldSettings {
                     Required = false,
                     Hint = Common.BulkActionCreateTaskHint + " Leave blank to use default task defined in settings."
                  })
              ).WithField("BulkActionWriteTask", field => field
                  .OfType(nameof(TextField))
                  .WithDisplayName("Bulk Action Write Task")
                  .WithPosition("13")
                  .WithSettings(new TextFieldSettings {
                     Required = false,
                     Hint = Common.BulkActionWriteTaskHint + " Leave blank to use default task defined in settings."
                  })
              ).WithField("BulkActionSummaryTask", field => field
                  .OfType(nameof(TextField))
                  .WithDisplayName("Bulk Action Summary Task")
                  .WithPosition("14")
                  .WithSettings(new TextFieldSettings {
                     Required = false,
                     Hint = Common.BulkActionSummaryTaskHint + " Leave blank to use default task defined in settings."
                  })
              ).WithField("BulkActionRunTask", field => field
                  .OfType(nameof(TextField))
                  .WithDisplayName("Bulk Action Run Task")
                  .WithPosition("15")
                  .WithSettings(new TextFieldSettings {
                     Required = false,
                     Hint = Common.BulkActionRunTaskHint + " Leave blank to use default task defined in settings."
                  })
              ).WithField("BulkActionSuccessTask", field => field
                  .OfType(nameof(TextField))
                  .WithDisplayName("Bulk Action Success Task")
                  .WithPosition("16")
                  .WithSettings(new TextFieldSettings {
                     Required = false,
                     Hint = Common.BulkActionSuccessTaskHint + " Leave blank to use default task defined in settings."
                  })
              ).WithField("BulkActionFailTask", field => field
                  .OfType(nameof(TextField))
                  .WithDisplayName("Bulk Action Fail Task")
                  .WithPosition("17")
                  .WithSettings(new TextFieldSettings {
                     Required = false,
                     Hint = Common.BulkActionFailTaskHint + " Leave blank to use default task defined in settings."
                  })
              )
         );

         _contentDefinitionManager.AlterTypeDefinition("TransformalizeReport", builder => builder
             .Creatable()
             .Listable()
             .WithPart("TitlePart", part => part.WithPosition("1"))
             .WithPart("AliasPart", part => part
                 .WithPosition("2")
                 .WithSettings(new AliasPartSettings {
                    Pattern = "{{ ContentItem | title | slugify }}"
                 })
             )
             .WithPart("TransformalizeReportPart", part => part.WithPosition("3"))
             .WithPart("CommonPart", part => part.WithPosition("4"))
         );

         _contentDefinitionManager.AlterPartDefinition("TransformalizeTaskPart", part => part
             .WithDisplayName("Transformalize Task Part")
             .WithDescription("Provides fields for Transformalize Task content type")
             .WithField("Arrangement", field => field
                 .OfType(nameof(TextField))
                 .WithDisplayName("Arrangement")
                 .WithPosition("3")
                 .WithEditor("TransformalizeArrangement")
                 .WithSettings(new TextFieldSettings {
                    Hint = string.Empty,
                    Required = true
                 }
               )
            )
         );

         _contentDefinitionManager.AlterTypeDefinition("TransformalizeTask", builder => builder
             .Creatable()
             .Listable()
             .WithPart("TitlePart", part => part.WithPosition("1"))
             .WithPart("AliasPart", part => part
                 .WithPosition("2")
                 .WithSettings(new AliasPartSettings {
                    Pattern = "{{ ContentItem | title | slugify }}"
                 })
             )
             .WithPart("TransformalizeTaskPart", part => part.WithPosition("3"))
             .WithPart("CommonPart", part => part.WithPosition("4"))
         );

         return 1;
      }

      public int UpdateFrom1() {
         _contentDefinitionManager.AlterPartDefinition("TransformalizeReportPart", part => part
          .WithField("Map", field => field
             .OfType(nameof(BooleanField))
             .WithDisplayName("Map")
             .WithPosition("20")
             .WithSettings(new BooleanFieldSettings {
                DefaultValue = false,
                Hint = "Allow user to view a map of the report's records.",
                Label = "Map"
             }
             )
           )
          .WithField("MapLatitudeField", field => field
               .OfType(nameof(TextField))
               .WithDisplayName("Map Latititude Field")
               .WithPosition("21")
               .WithSettings(new TextFieldSettings {
                  Required = false,
                  Hint = "This field's value is used as the record's latitude on the map."
               })
           ).WithField("MapLongitudeField", field => field
               .OfType(nameof(TextField))
               .WithDisplayName("Map Longitude Field")
               .WithPosition("22")
               .WithSettings(new TextFieldSettings {
                  Required = false,
                  Hint = "This field's value is used as the record's longitude on the map."
               })
            ).WithField("MapDescriptionField", field => field
               .OfType(nameof(TextField))
               .WithDisplayName("Map Description Field")
               .WithPosition("23")
               .WithSettings(new TextFieldSettings {
                  Required = false,
                  Hint = "This field's content is placed in the GEO JSON Description property, and is used in the pop up.  This is usually an HTML snippet you want to display."
               })
           ).WithField("MapColorField", field => field
             .OfType(nameof(TextField))
             .WithDisplayName("Map Color Field")
             .WithPosition("24")
             .WithSettings(new TextFieldSettings {
                Required = false,
                Hint = "Specify a field to control the color of the dots on the map.  Or, specify a hex representation of a color (e.g. #ffc0cb)."
             }
             )
           )
         );

         return 2;
      }

      public int UpdateFrom2() {
         _contentDefinitionManager.AlterPartDefinition("TransformalizeReportPart", part => part
          .WithField("MapRadiusField", field => field
             .OfType(nameof(TextField))
             .WithDisplayName("Map Radius Field")
             .WithPosition("25")
             .WithSettings(new TextFieldSettings {
                Required = false,
                Hint = "Specify a field to control the radius of the dots on the map.  Or, just set an integer value (e.g. 7)."
             }
             )
           ).WithField("MapOpacityField", field => field
             .OfType(nameof(TextField))
             .WithDisplayName("Map Opacity Field")
             .WithPosition("26")
             .WithSettings(new TextFieldSettings {
                Required = false,
                Hint = "Specify a field to control the opacity of the dots on the map.  Or, just set a numeric value between 0.1 and 1.0 (e.g. 0.8)."
             }
             )
           )
         );

         return 3;
      }

      public int UpdateFrom3() {
         _contentDefinitionManager.AlterPartDefinition("TransformalizeReportPart", part => part
          .WithField("PageSizesExtended", field => field
                 .OfType(nameof(TextField))
                 .WithDisplayName("Page Sizes Extended")
                 .WithPosition("5")
                 .WithSettings(new TextFieldSettings {
                    Required = false,
                    Hint = "To overide default extended page sizes, specify a comma delimited list of page sizes (integers). To use the common extended page sizes defined in settings, leave it blank. To disable pagination for calendar or map, set this to 0."
                 }
                )
             )
         );

         return 4;
      }

      public int UpdateFrom4() {
         _contentDefinitionManager.AlterPartDefinition("TransformalizeReportPart", part => part
         .WithField("Calendar", field => field
                .OfType(nameof(BooleanField))
                .WithDisplayName("Calendar")
                .WithPosition("30")
                .WithSettings(new BooleanFieldSettings {
                   DefaultValue = false,
                   Hint = "Allow user to view calendar?",
                   Label = "Calendar"
                }
                )
              )
          .WithField("CalendarIdField", field => field
             .OfType(nameof(TextField))
             .WithDisplayName("Calendar Id Field")
             .WithPosition("31")
             .WithSettings(new TextFieldSettings {
                Required = false,
                Hint = "Specify the field that contains a unique id for each calendar item."
             }
             )
           ).WithField("CalendarTitleField", field => field
             .OfType(nameof(TextField))
             .WithDisplayName("Calendar Title Field")
             .WithPosition("32")
             .WithSettings(new TextFieldSettings {
                Required = false,
                Hint = "Specify the field that contains a title for each calendar item."
             }
             )
           ).WithField("CalendarUrlField", field => field
             .OfType(nameof(TextField))
             .WithDisplayName("Calendar URL Field")
             .WithPosition("33")
             .WithSettings(new TextFieldSettings {
                Required = false,
                Hint = "Specify the field that contains a title for each calendar item."
             }
             )
           ).WithField("CalendarClassField", field => field
             .OfType(nameof(TextField))
             .WithDisplayName("Calendar Class Field")
             .WithPosition("34")
             .WithSettings(new TextFieldSettings {
                Required = false,
                Hint = "Specify the field that contains a CSS class for each calendar item.  Current implementation expects event-important, event-success, event-warning, event-info, event-inverse or event-special."
             }
             )
           ).WithField("CalendarStartField", field => field
             .OfType(nameof(TextField))
             .WithDisplayName("Calendar Start Field")
             .WithPosition("35")
             .WithSettings(new TextFieldSettings {
                Required = false,
                Hint = "Specify the field that contains the starting time for each calendar item.  Current implementation expects a UTC date/time converted to unix time (a long integer indicatng milliseconds since 1970-01-01)."
             }
             )
           ).WithField("CalendarEndField", field => field
             .OfType(nameof(TextField))
             .WithDisplayName("Calendar End Field")
             .WithPosition("36")
             .WithSettings(new TextFieldSettings {
                Required = false,
                Hint = "Specify the field that contains the ending time for each calendar item.  Current implementation expects a UTC date/time converted to unix time (a long integer indicatng milliseconds since 1970-01-01)."
             }
             )
           )
         );

         return 5;
      }

      public int UpdateFrom5() {

         _contentDefinitionManager.AlterPartDefinition("TransformalizeFormPart", part => part
             .WithDisplayName("Transformalize Form Part")
             .WithDescription("Provides fields for Transformalize Form content type")
             .WithField("Arrangement", field => field
                 .OfType(nameof(TextField))
                 .WithDisplayName("Arrangement")
                 .WithPosition("3")
                 .WithEditor("TransformalizeArrangement")
                 .WithSettings(new TextFieldSettings {
                    Hint = string.Empty,
                    Required = true
                 }
               )
            )
         );

         _contentDefinitionManager.AlterTypeDefinition("TransformalizeForm", builder => builder
             .Creatable()
             .Listable()
             .WithPart("TitlePart", part => part.WithPosition("1"))
             .WithPart("AliasPart", part => part
                 .WithPosition("2")
                 .WithSettings(new AliasPartSettings {
                    Pattern = "{{ ContentItem | title | slugify }}"
                 })
             )
             .WithPart("TransformalizeFormPart", part => part.WithPosition("3"))
             .WithPart("CommonPart", part => part.WithPosition("4"))
         );

         return 6;

      }

      public int UpdateFrom6() {

         _contentDefinitionManager.AlterPartDefinition("TransformalizeFilePart", part => part
             .WithDisplayName("Transformalize File Part")
             .WithDescription("Provides fields for Transformalize File content type")
             .WithField("OriginalName", field => field
                 .OfType(nameof(TextField))
                 .WithDisplayName("Original File Name")
                 .WithPosition("3")
                 .WithSettings(new TextFieldSettings {
                    Hint = "This is the original name of the file uploaded to the server.",
                    Required = true
                 }
                  )
            ).WithField("FullPath", field => field
               .OfType(nameof(TextField))
               .WithDisplayName("File Path")
               .WithPosition("4")
               .WithSettings(new TextFieldSettings {
                  Hint = "This is the path used to retrieve the file.  It is relative to the Transformalize/Files in the tenant's folder.",
                  Required = true
               }
               )
            )
         );

         _contentDefinitionManager.AlterTypeDefinition("TransformalizeFile", builder => builder
             .WithPart("AliasPart", part => part
                 .WithPosition("1")
                 .WithSettings(new AliasPartSettings {
                    Pattern = "{{ ContentItem | title | slugify }}"
                 })
             )
             .WithPart("TransformalizeFilePart", part => part.WithPosition("2"))
             .WithPart("CommonPart", part => part.WithPosition("3"))
         );

         return 7;

      }

      public int UpdateFrom7() {
         return 8;
      }

      public int UpdateFrom8() {

         _contentDefinitionManager.AlterPartDefinition("TransformalizeFormPart", part => part
         .WithField("LocationEnableHighAccuracy", field => field
                .OfType(nameof(BooleanField))
                .WithDisplayName("Enable High Accuracy")
                .WithPosition("11")
                .WithSettings(new BooleanFieldSettings {
                   DefaultValue = false,
                   Label = "Enable High Accuracy",
                   Hint = "Is a Boolean that indicates the application would like to receive the best possible results. If true and if the device is able to provide a more accurate position, it will do so. Note that this can result in slower response times or increased power consumption (with a GPS chip on a mobile device for example). On the other hand, if false, the device can take the liberty to save resources by responding more quickly and/or using less power. Default: false."
                }
                )
              )
          .WithField("LocationMaximumAge", field => field
             .OfType(nameof(NumericField))
             .WithDisplayName("Maximum Age")
             .WithPosition("12")
             .WithSettings(new NumericFieldSettings {
                Scale = 0,
                DefaultValue = "0",
                Minimum = -1,
                Maximum = int.MaxValue,
                Hint = "Is a positive long value indicating the maximum age in milliseconds of a possible cached position that is acceptable to return. If set to 0, it means that the device cannot use a cached position and must attempt to retrieve the real current position. If set to Infinity (-1) the device must return a cached position regardless of its age. Default: 0."
             }
             )
           ).WithField("LocationTimeout", field => field
             .OfType(nameof(NumericField))
             .WithDisplayName("Timeout")
             .WithPosition("13")
             .WithSettings(new NumericFieldSettings {
                Scale = 0,
                DefaultValue = "-1",
                Minimum = -1,
                Maximum = int.MaxValue,
                Hint = "Is a positive long value representing the maximum length of time (in milliseconds) the device is allowed to take in order to return a position. The default value is Infinity (-1), meaning that getCurrentPosition() won't return until the position is available."
             }
             )
           )
         );

         return 9;
      }

      private async Task EnableFeature(string id) {

         var availableFeatures = await _moduleService.GetAvailableFeaturesAsync();

         var contentFields = availableFeatures.FirstOrDefault(f => f.Descriptor.Id == id);
         if (contentFields != null) {
            if (!contentFields.IsEnabled) {
               _logger.LogInformation($"Enabling {id}");
               await _moduleService.EnableFeaturesAsync(new[] { id });
            }
         } else {
            _logger.LogError($"Unable to find {id} features required for Transformalize.");
         }
      }
   }
}
