﻿using Dapper;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using YesSql;
using YesSql.Sql;
using YesSql.Sql.Schema;

namespace EasyOC.OrchardCore.DynamicTypeIndex
{
    public class DynamicIndexTableBuilder : IDynamicIndexTableBuilder
    {
        private ICommandInterpreter _commandInterpreter;
        private readonly ILogger _logger;

        public string TablePrefix { get; private set; }
        public ISqlDialect Dialect { get; private set; }
        public ITableNameConvention TableNameConvention { get; private set; }
        public DbConnection Connection { get; private set; }
        public DbTransaction Transaction { get; private set; }
        public bool ThrowOnError { get; private set; }

        public DynamicIndexTableBuilder(IConfiguration configuration, DbTransaction transaction, bool throwOnError = true)
        {
            Transaction = transaction;
            _logger = configuration.Logger;
            Connection = Transaction.Connection;
            _commandInterpreter = configuration.CommandInterpreter;
            Dialect = configuration.SqlDialect;
            TablePrefix = configuration.TablePrefix;
            ThrowOnError = throwOnError;
            TableNameConvention = configuration.TableNameConvention;
        }

        private void Execute(IEnumerable<string> statements)
        {
            foreach (var statement in statements)
            {
                _logger.LogTrace(statement);
                Connection.Execute(statement, null, Transaction);
            }
        }

        private string Prefix(string table)
        {
            return TablePrefix + table;
        }
        private string GetIndexTable(string typeName, string collection = null)
        {
            if (String.IsNullOrEmpty(collection))
            {
                return typeName;
            }

            return collection + "_" + typeName;

        }
        public IDynamicIndexTableBuilder CreateMapIndexTable(ContentTypeDefinition contentType, Action<ICreateTableCommand> table, string collection)
        {
            try
            {
                var indexName = contentType.Name;
                var typeName = this.GetIndexTable(indexName, collection);
                var createTable = new CreateTableCommand(Prefix(typeName));
                var documentTable = TableNameConvention.GetDocumentTable(collection);

                // NB: Identity() implies PrimaryKey()

                createTable
                    .Column<int>("Id", column => column.Identity().NotNull())
                    .Column<int>("DocumentId")
                    ;

                table(createTable);
                Execute(_commandInterpreter.CreateSql(createTable));

                CreateForeignKey("FK_" + (collection ?? "") + indexName, typeName, new[] { "DocumentId" }, documentTable, new[] { "Id" });

                AlterTable(typeName, table =>
                    table.CreateIndex($"IDX_FK_{typeName}", "DocumentId")
                    );
            }
            catch
            {
                if (ThrowOnError)
                {
                    throw;
                }
            }

            return this;
        }

        public IDynamicIndexTableBuilder CreateReduceIndexTable(Type indexType, Action<ICreateTableCommand> table, string collection = null)
        {
            try
            {
                var indexName = indexType.Name;
                var indexTable = TableNameConvention.GetIndexTable(indexType, collection);
                var createTable = new CreateTableCommand(Prefix(indexTable));
                var documentTable = TableNameConvention.GetDocumentTable(collection);

                // NB: Identity() implies PrimaryKey()

                createTable
                    .Column<int>("Id", column => column.Identity().NotNull())
                    ;

                table(createTable);
                Execute(_commandInterpreter.CreateSql(createTable));

                var bridgeTableName = indexTable + "_" + documentTable;

                CreateTable(bridgeTableName, bridge => bridge
                    .Column<int>(indexName + "Id", column => column.NotNull())
                    .Column<int>("DocumentId", column => column.NotNull())
                );

                CreateForeignKey("FK_" + bridgeTableName + "_Id", bridgeTableName, new[] { indexName + "Id" }, indexTable, new[] { "Id" });
                CreateForeignKey("FK_" + bridgeTableName + "_DocumentId", bridgeTableName, new[] { "DocumentId" }, documentTable, new[] { "Id" });

                AlterTable(bridgeTableName, table =>
                    table.CreateIndex($"IDX_FK_{bridgeTableName}", indexName + "Id", "DocumentId")
                    );
            }
            catch
            {
                if (ThrowOnError)
                {
                    throw;
                }
            }

            return this;
        }

        public IDynamicIndexTableBuilder DropReduceIndexTable(Type indexType, string collection = null)
        {
            try
            {
                var indexTable = TableNameConvention.GetIndexTable(indexType, collection);
                var documentTable = TableNameConvention.GetDocumentTable(collection);

                var bridgeTableName = indexTable + "_" + documentTable;

                if (String.IsNullOrEmpty(Dialect.CascadeConstraintsString))
                {
                    DropForeignKey(bridgeTableName, "FK_" + bridgeTableName + "_Id");
                    DropForeignKey(bridgeTableName, "FK_" + bridgeTableName + "_DocumentId");
                }

                DropTable(bridgeTableName);
                DropTable(indexTable);
            }
            catch
            {
                if (ThrowOnError)
                {
                    throw;
                }
            }

            return this;
        }

        public IDynamicIndexTableBuilder DropMapIndexTable(Type indexType, string collection = null)
        {
            try
            {
                var indexName = indexType.Name;
                var indexTable = TableNameConvention.GetIndexTable(indexType, collection);

                if (String.IsNullOrEmpty(Dialect.CascadeConstraintsString))
                {
                    DropForeignKey(indexTable, "FK_" + (collection ?? "") + indexName);
                }

                DropTable(indexTable);
            }
            catch
            {
                if (ThrowOnError)
                {
                    throw;
                }
            }

            return this;
        }

        public IDynamicIndexTableBuilder CreateTable(string name, Action<ICreateTableCommand> table)
        {
            try
            {
                var createTable = new CreateTableCommand(Prefix(name));
                table(createTable);
                Execute(_commandInterpreter.CreateSql(createTable));
            }
            catch
            {
                if (ThrowOnError)
                {
                    throw;
                }
            }

            return this;
        }

        public IDynamicIndexTableBuilder AlterTable(string name, Action<IAlterTableCommand> table)
        {
            try
            {
                var alterTable = new AlterTableCommand(Prefix(name), Dialect, TablePrefix);
                table(alterTable);
                Execute(_commandInterpreter.CreateSql(alterTable));
            }
            catch
            {
                if (ThrowOnError)
                {
                    throw;
                }
            }

            return this;
        }

        public IDynamicIndexTableBuilder AlterIndexTable(Type indexType, Action<IAlterTableCommand> table, string collection)
        {
            var indexTable = TableNameConvention.GetIndexTable(indexType, collection);
            AlterTable(indexTable, table);

            return this;
        }

        public IDynamicIndexTableBuilder DropTable(string name)
        {
            try
            {
                var deleteTable = new DropTableCommand(Prefix(name));
                Execute(_commandInterpreter.CreateSql(deleteTable));
            }
            catch
            {
                if (ThrowOnError)
                {
                    throw;
                }
            }

            return this;
        }

        public IDynamicIndexTableBuilder CreateForeignKey(string name, string srcTable, string[] srcColumns, string destTable, string[] destColumns)
        {
            try
            {
                var command = new CreateForeignKeyCommand(Dialect.FormatKeyName(Prefix(name)), Prefix(srcTable), srcColumns, Prefix(destTable), destColumns);
                var sql = _commandInterpreter.CreateSql(command);
                Execute(sql);
            }
            catch
            {
                if (ThrowOnError)
                {
                    throw;
                }
            }

            return this;
        }

        public IDynamicIndexTableBuilder DropForeignKey(string srcTable, string name)
        {
            try
            {
                var command = new DropForeignKeyCommand(Dialect.FormatKeyName(Prefix(srcTable)), Prefix(name));
                Execute(_commandInterpreter.CreateSql(command));
            }
            catch
            {
                if (ThrowOnError)
                {
                    throw;
                }
            }

            return this;
        }


    }
}
