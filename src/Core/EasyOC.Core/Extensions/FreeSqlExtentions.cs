using EasyOC.Core.Indexs;
using FreeSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Shells.Database.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
namespace System
{
    public static class FreeSqlExtentions
    {
        static IdleBus<IFreeSql> ib = new IdleBus<IFreeSql>(TimeSpan.FromMinutes(10));

        public static IServiceCollection AddFreeSql(this IServiceCollection services)
        {

            return services.AddSingleton(serviceProvider => serviceProvider.GetFreeSql());
        }

        public static IFreeSql GetFreeSql(this IServiceProvider serviceProvider)
        {

            var shellConfig = serviceProvider.GetRequiredService<IShellConfiguration>();
            var dbOptions = shellConfig.Get<DatabaseShellsStorageOptions>();
            var targetDbType = ConvertToFreeSqlDataType(dbOptions.DatabaseProvider);

            if (targetDbType == DataType.Sqlite)
            {
                var sqliteConnectionString = GetSqliteConnectionString(serviceProvider);
                var fsql = serviceProvider.GetFreeSql(targetDbType, sqliteConnectionString, dbOptions.TablePrefix);
                return fsql;
            }
            else
            {
                var fsql = serviceProvider.GetFreeSql(targetDbType, dbOptions.ConnectionString, dbOptions.TablePrefix);
                return fsql;
            }
        }

        public static IFreeSql GetFreeSql(this IServiceProvider serviceProvider, string providerName, string connectionString, string tablePrefix = default)
        {
            return serviceProvider.GetFreeSql(Enum.Parse<DataType>(providerName), connectionString, tablePrefix);
        }


        public static IFreeSql GetFreeSql(this IServiceProvider serviceProvider, DataType dataType, string connectionString, string tablePrefix = default)
        {

            var logger = serviceProvider.GetService<ILogger<FreeSqlBuilder>>();

            var ibKey = $"{connectionString}_{tablePrefix}";
            if (ib.Exists(ibKey))
            {
                var fsq = ib.Get(ibKey);
                if (fsq.Ado.MasterPool.IsAvailable && fsq.Ado.ExecuteConnectTest())
                {
                    return fsq;
                }
                else
                {
                    ib.TryRemove(ibKey, true);
                }
            }


            //按照需要添加其他数据库的引用 
            ib.Register(ibKey,
                () =>
                {
                    var fsql = new FreeSqlBuilder().UseConnectionString(dataType, connectionString)
                                       .UseMonitorCommand(executing =>
                                       {
                                           executing.CommandTimeout = 6000;

                                       }, executed: (cmd, traceLog) =>
                                       {
                                           var logStr = new StringBuilder();
                                           if (cmd.Parameters.Count > 0)
                                           {
                                               logStr.AppendLine($"--Parameters: \r\ndeclare ");
                                               var tempArray = new List<string>();
                                               foreach (DbParameter item in cmd.Parameters)
                                               {
                                                   tempArray.Add($"\t{item.ParameterName} {item.SourceColumn}='{item.Value}'");
                                               }
                                               logStr.AppendLine(string.Join(",\r\n", tempArray));
                                           }

                                           logStr.AppendLine($"\n{traceLog}\r\n");

                                           var result = logStr.ToString();
                                           Console.WriteLine(result);
                                           if (logger != null)
                                           {
                                               if (logger.IsEnabled(LogLevel.Debug))
                                               {
                                                   logger.LogDebug(result);
                                               }
                                           }
                                       })
                                       .Build();

                    fsql.Aop.ConfigEntity += (s, e) =>
                    {

                        var tableName = e.EntityType.Name;
                        if (e.ModifyResult != null && !e.ModifyResult.Name.IsNullOrEmpty())
                        {
                            tableName = e.ModifyResult.Name;
                            var attr = e.EntityType.GetCustomAttributes().FirstOrDefault(x => x is EOCTableAttribute);
                            if (attr is EOCTableAttribute tableAttribute)
                            {
                                tableName = string.Format("{0}_{1}", tableAttribute.Collection, tableName);
                            }
                        }
                        if (!string.IsNullOrEmpty(tablePrefix))
                        {
                            e.ModifyResult.Name = string.Format("{0}_{1}", tablePrefix, tableName); //表名前缀
                        }
                        else
                        {
                            e.ModifyResult.Name = tableName;
                        }
                    };
                    return fsql;
                }


               );

            return ib.Get(ibKey);
        }

        public static string GetSqliteConnectionString(this IServiceProvider serviceProvider)
        {
            var shellOptions = serviceProvider.GetService<IOptions<ShellOptions>>();
            var option = shellOptions.Value;
            var shellSettings = serviceProvider.GetService<ShellSettings>();
            var databaseFolder = Path.Combine(option.ShellsApplicationDataPath, option.ShellsContainerName, shellSettings.Name);
            var databaseFile = Path.Combine(databaseFolder, "yessql.db");
            return $"Data Source={databaseFile};Cache=Shared";
        }



        public static DataType ConvertToFreeSqlDataType(string providerName)

        {
            if (!string.IsNullOrEmpty(providerName))
            {
                switch (providerName)
                {
                    //以下与FreeSql 不同,单独指定
                    case "SqlConnection":
                        return DataType.SqlServer;
                    case "Postgres":
                        return DataType.PostgreSQL;
                    //以下与FreeSql 相同
                    //case "Sqlite":
                    //    return DataType.Sqlite;
                    //case "MySql":
                    //    return DataType.MySql;
                    //其他
                    default:
                        DataType dataType;
                        if (Enum.TryParse(providerName.Replace(" ", string.Empty), out dataType))
                        {
                            return dataType;
                        }
                        break;
                }
            }
            throw new ArgumentException("未识别 或 FreeSql 尚未支持的数据库类型:" + providerName);
        }
    }
}



