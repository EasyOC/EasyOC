using OrchardCore.ContentManagement.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YesSql.Sql;
using YesSql.Sql.Schema;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Extentions
{
    public static class SchemaExtentions
    {
        public static ISchemaBuilder CreateMapIndexTable(this ISchemaBuilder builder,
            ContentTypeDefinition contentType,
            Action<ICreateTableCommand> table, string collection)
        {
            try
            {
                var indexName = contentType.Name;
                var tableName = GetIndexTable(indexName, collection);

                builder.CreateTable(tableName, (createTable) =>
                {
                    // NB: Identity() implies PrimaryKey()
                    createTable
                           .Column<int>("Id", column => column.Identity().NotNull())
                           .Column<int>("DocumentId")
                           ;
                    table(createTable);
                });
                var documentTable = builder.TableNameConvention.GetDocumentTable(collection);

                builder.CreateForeignKey("FK_" + (collection ?? "") + indexName, tableName, new[] { "DocumentId" }, documentTable, new[] { "Id" });

                builder.AlterTable(tableName, table =>
                    table.CreateIndex($"IDX_FK_{tableName}", "DocumentId")
                    );
            }
            catch
            {
                if (builder.ThrowOnError)
                {
                    throw;
                }
            }

            return builder;
        }

        private static string GetIndexTable(string typeName, string collection = null)
        {
            if (String.IsNullOrEmpty(collection))
            {
                return typeName;
            }

            return collection + "_" + typeName;

        }


    }
}
