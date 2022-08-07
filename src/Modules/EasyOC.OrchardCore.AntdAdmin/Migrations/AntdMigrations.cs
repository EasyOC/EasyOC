using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Data.Migration;

namespace EasyOC.OrchardCore.AntdAdmin.Migrations
{
    public class AntdMigrations: DataMigration
    {

            private readonly IContentDefinitionManager _contentDefinitionManager;
            public AntdMigrations(IContentDefinitionManager contentDefinitionManager)
            {
                _contentDefinitionManager = contentDefinitionManager;

            }


    }
}
