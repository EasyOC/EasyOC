using OrchardCore.ContentManagement.Metadata.Models;
using System;
using YesSql.Sql.Schema;

namespace YesSql.Sql
{
    public static class SchemaExtentions
    {
        public static string GetIndexTableName(this ISchemaBuilder builder, string tableName)
        {
            return builder.TablePrefix + tableName;
        }
        public static ISchemaBuilder CreateMapIndexTable(this ISchemaBuilder builder,
            string indexName,
            Action<ICreateTableCommand> table, string collection = "")
        {
            try
            {

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
