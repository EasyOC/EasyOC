using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using OrchardCore.Data.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Migrations
{
    public class DynamicIndexDataMigration : DataMigration
    {
        private readonly IFreeSql _freeSql;

        public DynamicIndexDataMigration(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        public int Create()
        {
            _freeSql.CodeFirst.SyncStructure<DynamicIndexConfigDataIndex>();
            SchemaBuilder.CreateForeignKey<DynamicIndexConfigDataIndex>();
            
            return 1;
        }
    }
}
