using EasyOC.OrchardCore.RDBMS.Services;
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
using System.Text;

namespace EasyOC.OrchardCore.RDBMS.DataAccess
{
    public static class FreeSqlExtentions
    {
        public static IServiceCollection AddFreeSql(this IServiceCollection services)
        {

            return services.AddSingleton(serviceProvider =>
            {

                var shellOptions = serviceProvider.GetService<IOptions<ShellOptions>>();
                var option = shellOptions.Value;
                var shellSettings = serviceProvider.GetService<ShellSettings>();
                var databaseFolder = Path.Combine(option.ShellsApplicationDataPath, option.ShellsContainerName, shellSettings.Name);
                var databaseFile = Path.Combine(databaseFolder, "yessql.db");
                var shellConfig = serviceProvider.GetRequiredService<IShellConfiguration>();
                var dbOptions = shellConfig.Get<DatabaseShellsStorageOptions>();
                var targetDbType = ConvertToFreeSqlDataType(dbOptions.DatabaseProvider);

                var logger = serviceProvider.GetService<ILogger<FreeSqlBuilder>>();
                var fsql = FreeSqlProviderFactory.GetFreeSql(targetDbType, $"Data Source={databaseFile};Cache=Shared", logger);
                return fsql;
            });
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



