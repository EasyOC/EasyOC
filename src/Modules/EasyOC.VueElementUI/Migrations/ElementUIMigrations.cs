using OrchardCore.Autoroute.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace EasyOC.VueElementUI.Migrations
{
    public class ElementUIMigrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ElementUIMigrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public int Create()
        {
            _contentDefinitionManager.AlterTypeDefinition("VueForm", type =>
                type
                .Draftable()
                .Versionable()
                .Securable()
                .WithPart("AutoroutePart", part => part
                    .WithSettings(new AutoroutePartSettings
                    {
                        AllowCustomPath = true,
                        ShowHomepageOption = true,
                    })
                )
            );


            return 1;
        }
    }
}



