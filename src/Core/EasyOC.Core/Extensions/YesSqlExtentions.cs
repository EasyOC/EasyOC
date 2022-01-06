using EasyOC.Core.Indexs;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Scope;
using System;
using YesSql;
using YesSql.Sql;

namespace EasyOC
{
    public static class YesSqlExtentions
    {
        public static IQuery<T> Page<T>(this IQuery<T> source, PageReqest input)
            where T : class
        {
            return source.Skip(input.GetStartIndex()).Take(input.PageSize);
        }


        public static ISchemaBuilder CreateMapIndexTable<IndexTable>(this ISchemaBuilder builder, string collection = null)
            where IndexTable : class, IFreeSqlMapDocumentIndex
        {

            var fsql = ShellScope.Current.ServiceProvider.GetRequiredService<IFreeSql>();
            try
            {
                fsql.CodeFirst.SyncStructure<IndexTable>();
            }
            catch
            {
                throw;
            }

            return builder;

        }

        public static ISchemaBuilder CreateForeignKey<IndexTable>(this ISchemaBuilder builder, string collection = null)
            where IndexTable : class, IFreeSqlMapDocumentIndex
        {
            var indexType = typeof(IndexTable);
            var indexName = indexType.Name;
            var indexTable = builder.TableNameConvention.GetIndexTable(indexType, collection);
            var documentTable = builder.TableNameConvention.GetDocumentTable(collection);
            builder.CreateForeignKey("FK_" + (collection ?? "") + indexName, indexTable, new[] { "DocumentId" }, documentTable, new[] { "Id" });
            builder.AlterTable(indexTable, table =>
                    table.CreateIndex($"IDX_FK_{indexTable}", "DocumentId")
                );
            return builder;
        }

    }
}
