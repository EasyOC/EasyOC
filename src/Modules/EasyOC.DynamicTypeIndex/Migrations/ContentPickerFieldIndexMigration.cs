using EasyOC.DynamicTypeIndex.Indexing;
using OrchardCore.ContentFields.Indexing.SQL;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Data.Migration;
using YesSql.Sql;

namespace EasyOC.DynamicTypeIndex.Migrations
{
    public class ContentPickerFieldIndexMigration : DataMigration
    {
        //private readonly IFreeSql _freeSql;

        //public DynamicIndexDataMigration(IFreeSql freeSql)
        //{
        //    _freeSql = freeSql;
        //}

        public int Create()
        {
            //_freeSql.CodeFirst.SyncStructure<DynamicIndexConfigDataIndex>();

            #region ContentPickerFieldIndex
            SchemaBuilder.CreateMapIndexTable<ContentPickerFieldIndex>(table => table
                .Column<string>("ContentItemId", column => column.WithLength(26))
                .Column<string>("ContentItemVersionId", column => column.WithLength(26))
                .Column<string>("ContentType", column => column.WithLength(ContentItemIndex.MaxContentTypeSize))
                .Column<string>("ContentPart", column => column.WithLength(ContentItemIndex.MaxContentPartSize))
                .Column<string>("ContentField", column => column.WithLength(ContentItemIndex.MaxContentFieldSize))
                .Column<bool>("Published", column => column.Nullable())
                .Column<bool>("Latest", column => column.Nullable())
                .Column<string>("SelectedContentItemId", column => column.WithLength(26))
            );

            SchemaBuilder.AlterIndexTable<ContentPickerFieldIndex>(table => table
                .CreateIndex("IDX_ContentPickerFieldIndex_DocumentId",
                "DocumentId",
                "ContentItemId",
                "ContentItemVersionId",
                "Published",
                "Latest")
            );

            SchemaBuilder.AlterIndexTable<ContentPickerFieldIndex>(table => table
                .CreateIndex("IDX_ContentPickerFieldIndex_DocumentId_ContentType",
                "DocumentId",
                "ContentType",
                "ContentPart",
                "ContentField",
                "Published",
                "Latest")
            );

            SchemaBuilder.AlterIndexTable<ContentPickerFieldIndex>(table => table
                .CreateIndex("IDX_ContentPickerField_DocumentId_SelectedItemId",
                "DocumentId",
                "SelectedContentItemId",
                "Published",
                "Latest")
            );
            #endregion

            return 1;
        }


    }
}
