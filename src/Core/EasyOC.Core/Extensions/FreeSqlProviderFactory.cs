using EasyOC.Core.Indexs;
using FreeSql;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace EasyOC
{
    public class FreeSqlProviderFactory
    {
        static IdleBus<IFreeSql> ib = new IdleBus<IFreeSql>(TimeSpan.FromMinutes(10));

        public static IFreeSql GetFreeSql(string providerName, string connectionString, ILogger logger = null, string tablePrefix = default)
        {
            return GetFreeSql(Enum.Parse<DataType>(providerName), connectionString, logger, tablePrefix);
        }
        public static IFreeSql GetFreeSql(DataType dataType, string connectionString, ILogger logger = null, string tablePrefix = default)
        {
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
                    var fsql = new FreeSqlBuilder().UseConnectionString(dataType, ibKey)
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
                        if (!string.IsNullOrEmpty(tablePrefix))
                        {
                            var tableName = e.EntityType.Name;

                            if (e.ModifyResult is EOCTableAttribute tableAttribute)
                            {
                                tableName = string.Format("{0}_{1}", tableAttribute.Collection, tableName);
                            }
                            e.ModifyResult.Name = string.Format("{0}_{1}", tablePrefix, tableName); //表名前缀
                        }
                    };
                    return fsql;
                }


               );

            return ib.Get(ibKey);
        }
    }
}



