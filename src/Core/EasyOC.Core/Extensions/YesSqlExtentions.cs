using EasyOC.Core.Indexes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using OrchardCore.Environment.Shell.Scope;
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




        //public static ISchemaBuilder CreateMapIndexTable<IndexTable>(this ISchemaBuilder builder, string collection = null)
        //    where IndexTable : class, IFreeSqlMapDocumentIndex
        //{
        //    var provider = ShellScope.Current.ServiceProvider;
        //    var fsql = provider.GetRequiredService<IFreeSql>();
        //    fsql.CodeFirst.SyncStructure<IndexTable>();
        //    return builder;
        //}

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
