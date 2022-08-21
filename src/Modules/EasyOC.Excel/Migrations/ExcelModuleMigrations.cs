using EasyOC.Excel.Models;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace EasyOC.Excel.Migrations
{
    public class ExcelModuleMigrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;


        public ExcelModuleMigrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;

        }


        public int Create()
        {
            Init();
            return 1;
        }

        public void Init()
        {

            _contentDefinitionManager.AlterPartDefinition(nameof(ImportExcelSettings), part => part
                .WithField("SheetName", field => field
                    .OfType("TextField")
                    .WithDisplayName("SheetName")
                    .WithPosition("0")
                )
                .WithField("StartRowNumber", field => field
                    .OfType("NumericField")
                    .WithDisplayName("Start Row Number")
                    .WithPosition("1")
                )
                .WithField("StartColumnName", field => field
                    .OfType("TextField")
                    .WithDisplayName("Start Column Name")
                    .WithPosition("2")
                )
                .WithField("EndColumnName", field => field
                    .OfType("TextField")
                    .WithDisplayName("End Column Name")
                    .WithPosition("3")
                )
                .WithField("TargetContentType", field => field
                    .OfType("TextField")
                    .WithDisplayName("TargetContentType")
                    .WithPosition("4")
                )
               .WithField("FieldsMappingConfig", field => field
                    .OfType("TextField")
                    .WithDisplayName("FieldsMappingConfig")
                    .WithEditor("Monaco")
                    .WithPosition("5")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Javascript",
                        Required = true,
                    })
                    .WithSettings(new TextFieldMonacoEditorSettings
                    {
                        Options = JObject.FromObject(new
                        {
                            automaticLayout = true,
                            language = "javascript"
                        }).ToString()
                    })
                )
            );
            _contentDefinitionManager.AlterTypeDefinition("ImportExcelSettings", type => type
                    .DisplayedAs("ImportExcelSettings")
                    .Creatable()
                    .Listable()
                    .Draftable()
                    .Versionable()
                    .Securable()
                    .WithPart(nameof(ImportExcelSettings), part => part
                        .WithPosition("0")
                    )
                    .WithPart("TitlePart")
                );


        }
    }
}



