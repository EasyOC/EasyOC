using FreeSql;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System;
using System.Linq;

namespace EasyOC.OrchardCore.RDBMS.Migrations
{
    public class DbConnectionConfigMigration : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public DbConnectionConfigMigration(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;

        }

        public int Create()
        {
            _contentDefinitionManager.AlterTypeDefinition("DbConnectionConfig", type => type
                   .DisplayedAs("DbConnectionConfig")
                   .Creatable()
                   .Listable()
                   .Draftable()
                   .Versionable()
                   .Securable()
                   .WithPart("DbConnectionConfig", part => part
                       .WithPosition("1")
                   )
                   .WithPart("TitlePart", part => part
                      .WithPosition("0")
                   )
               );
            var freeSqlProviders = Enum.GetNames<DataType>();
            _contentDefinitionManager.AlterPartDefinition("DbConnectionConfig", part => part
                .WithField("DatabaseProvider", field => field
                    .OfType("TextField")
                    .WithDisplayName("Database Provider")
                    .WithEditor("PredefinedList")
                    .WithPosition("0")
                    .WithSettings(new TextFieldPredefinedListEditorSettings
                    {
                        Options = freeSqlProviders
                        .Select(x => new ListValueOption { Name = x, Value = x })
                            .ToArray(),
                        Editor = EditorOption.Dropdown,
                        DefaultValue = "Sqlite"

                    }).WithSettings(
                    new TextFieldSettings()
                        {
                            Hint =@"FreeSql支持的所有数据库，已添加 MySql, Sqlite, SqlServer.
                                    其它数据库请手动添加包引用到项目中"
                        }
                    )
                )
                .WithField("ConnectionString", field => field
                    .OfType("TextField")
                    .WithEditor("TextArea")
                    .WithDisplayName("ConnectionString")
                    .WithPosition("1")
                )
);


            return 1;
        }
    }
}



