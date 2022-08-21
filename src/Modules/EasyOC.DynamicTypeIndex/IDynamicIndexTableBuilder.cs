using OrchardCore.ContentManagement.Metadata.Models;
using System;
using System.Data.Common;
using YesSql;
using YesSql.Sql;
using YesSql.Sql.Schema;

namespace EasyOC.DynamicTypeIndex
{
    public interface IDynamicIndexTableBuilder
    {
        DbConnection Connection { get; }
        ISqlDialect Dialect { get; }
        ITableNameConvention TableNameConvention { get; }
        string TablePrefix { get; }
        bool ThrowOnError { get; }
        DbTransaction Transaction { get; }

        IDynamicIndexTableBuilder AlterIndexTable(Type indexType, Action<IAlterTableCommand> table, string collection);
        IDynamicIndexTableBuilder AlterTable(string name, Action<IAlterTableCommand> table);
        IDynamicIndexTableBuilder CreateForeignKey(string name, string srcTable, string[] srcColumns, string destTable, string[] destColumns);
        IDynamicIndexTableBuilder CreateMapIndexTable(ContentTypeDefinition contentType, Action<ICreateTableCommand> table, string collection);
        IDynamicIndexTableBuilder CreateReduceIndexTable(Type indexType, Action<ICreateTableCommand> table, string collection = null);
        IDynamicIndexTableBuilder CreateTable(string name, Action<ICreateTableCommand> table);
        IDynamicIndexTableBuilder DropForeignKey(string srcTable, string name);
        IDynamicIndexTableBuilder DropMapIndexTable(Type indexType, string collection = null);
        IDynamicIndexTableBuilder DropReduceIndexTable(Type indexType, string collection = null);
        IDynamicIndexTableBuilder DropTable(string name);
    }
}
