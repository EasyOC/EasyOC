using EasyOC.Core.Indexs;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq.Expressions;
using YesSql;
using YesSql.Indexes;
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

            //var props = typeof(IndexTable).GetProperties();
            var fsql = FreeSqlProviderFactory.GetFreeSql(null, builder.Connection.ConnectionString, NullLogger.Instance, builder.TablePrefix);
            fsql.CodeFirst.SyncStructure<IndexTable>();
            // builder.CreateMapIndexTable<IndexTable>(table =>
            //{
            //    foreach (var item in props)
            //    {
            //        table.Column(item.Name, item.PropertyType);
            //    }
            //}
            //, collection);



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



        public static IQuery<T, TIndex> WhereIf<T,TIndex>(this IQuery<T, TIndex> query, bool condition, Expression<Func<TIndex, bool>> predicate)
            where T : class
            where TIndex : IIndex
        {
            if (condition)
            {
                return query.Where(predicate);
            }
            else
            {
                return query;
            }
        }

    }
}
