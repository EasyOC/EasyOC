using EasyOC.DynamicTypeIndex.Indexing;
using OrchardCore.ContentFields.Indexing.SQL;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Data.Migration;
using YesSql.Sql;

namespace EasyOC.DynamicTypeIndex.Migrations
{
    public class DynamicIndexDataMigration : DataMigration
    {
        //private readonly IFreeSql _freeSql;

        //public DynamicIndexDataMigration(IFreeSql freeSql)
        //{
        //    _freeSql = freeSql;
        //}

        public int Create()
        {
            //_freeSql.CodeFirst.SyncStructure<DynamicIndexConfigDataIndex>();

            SchemaBuilder.CreateMapIndexTable<DynamicIndexConfigDataIndex>(table => table
                .Column<string>("ContentItemId", c => c.WithLength(26))
                .Column<string>("ContainedContentItemId", c => c.WithLength(26))
                .Column<string>("TypeName", c => c.Unlimited())
                .Column<string>("TableName", c => c.Unlimited())
                .Column<string>("ConfigData", c => c.Unlimited())
            );

            return 1;
        } 
    }
}
