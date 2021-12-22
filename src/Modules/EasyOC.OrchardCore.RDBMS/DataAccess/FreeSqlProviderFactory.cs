using FreeSql;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace EasyOC.OrchardCore.RDBMS.Services
{
    public class FreeSqlProviderFactory
    {
        static IdleBus<IFreeSql> ib = new IdleBus<IFreeSql>(TimeSpan.FromMinutes(10));

        public static IFreeSql GetFreeSql(string providerName, string connectionString, ILogger logger = null)
        {
            return GetFreeSql(Enum.Parse<DataType>(providerName), connectionString, logger);
        }
        public static IFreeSql GetFreeSql(DataType dataType, string connectionString, ILogger logger = null)
        {

            if (ib.Exists(connectionString))
            {
                var fsq = ib.Get(connectionString);
                if (fsq.Ado.MasterPool.IsAvailable && fsq.Ado.ExecuteConnectTest())
                {
                    return fsq;
                }
                else
                {
                    ib.TryRemove(connectionString, true);
                }
            }


            //按照需要添加其他数据库的引用 
            ib.Register(connectionString,
                () => new FreeSqlBuilder().UseConnectionString(dataType, connectionString)
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
                    .Build()
               );

            return ib.Get(connectionString);
        }
    }
}



