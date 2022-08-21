using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace EasyOC.RDBMS.Migrations
{
    public class RDBMSMappingConfigMigration : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public RDBMSMappingConfigMigration(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;

        }

        public int Create()
        {
            _contentDefinitionManager.AlterTypeDefinition("RDBMSMappingConfig", type => type
                 .DisplayedAs("RDBMS Mapping Config")
                 .Creatable()
                 .Listable()
                 .Draftable()
                 .Versionable()
                 .Securable()
                 .WithPart("RDBMSMappingConfig", part => part
                     .WithPosition("1")
                 )
                 .WithPart("TitlePart", part => part
                     .WithPosition("0")
                 )
             );

            _contentDefinitionManager.AlterPartDefinition("RDBMSMappingConfig", part => part
                .WithField("TargetTable", field => field
                    .OfType("TextField")
                    .WithDisplayName("Target Table")
                    .WithPosition("0")
                )
                .WithField("ContentTypeName", field => field
                    .OfType("TextField")
                    .WithDisplayName("ContentTypeName")
                    .WithPosition("1")
                )
                .WithField("MappingData", field => field
                    .OfType("TextField")
                    .WithDisplayName("MappingData")
                    .WithEditor("CodeMirror")
                    .WithPosition("2")
                )
                .WithField("EnableAutoSync", field => field
                    .OfType("BooleanField")
                    .WithDisplayName("Enable Auto Sync")
                    .WithEditor("Switch")
                    .WithPosition("4")
                )
                .WithField("ReadOnly", field => field
                    .OfType("BooleanField")
                    .WithDisplayName("Read Only")
                    .WithEditor("Switch")
                    .WithPosition("5")
                )
            );



            return 1;
        }
    }
}



