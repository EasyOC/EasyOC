using EasyOC.RDBMS.Models;
using EasyOC.RDBMS.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace EasyOC.RDBMS
{
    public class ExternalDbProvider
    {
        private IFreeSql _freeSql;
        private readonly IServiceProvider _serviceProvider;
        private readonly ExternalDbConfig _externalDbConfig;
        private readonly ILogger _logger;
        public ExternalDbProvider(IServiceProvider serviceProvider, ExternalDbConfig config, ILogger logger)
        {
            _serviceProvider = serviceProvider;
            _externalDbConfig = config;
            _logger = logger;
        }

        private IFreeSql FreeSql
        {
            get
            {
                if (_freeSql == null)
                {
                    try
                    {
                        if (_externalDbConfig.Name != Constants.ShellDbName)
                        {
                            var rdbMsService = _serviceProvider.GetRequiredService<IRDBMSAppService>();
                            _freeSql = rdbMsService
                                .GetFreeSqlAsync(_externalDbConfig.ConnectionConfigId)
                                .GetAwaiter().GetResult();
                        }
                        else
                        {
                            _freeSql = _serviceProvider.GetFreeSql();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("数据库连接失败,UseShellDb:{Name},ConnectionId:{ConnectionId},error:{Error}",
                        _externalDbConfig.Name, _externalDbConfig.ConnectionConfigId, ex);
                    }

                }
                return _freeSql;
            }
        }

        public IEnumerable<object> GetRows(string cmdText, object parms = null)
        {
            var result = FreeSql.Ado.Query<object>(cmdText, parms);
            return result.ToArray();
        }

        /// <summary>
        /// 执行SQL返回对象集合，Query&lt;User&gt;("select * from user where age > @age", new { age = 25 })<para></para>
        /// 提示：parms 参数还可以传 Dictionary&lt;string, object&gt;
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parms"></param>
        public DataTable GetTable(string cmdText, object parms = null)
        {
            var result = FreeSql.Ado.ExecuteDataTable(cmdText, parms);
            return result;
        }

        public DataSet GetDataSet(string cmdText, object parms = null)
            => FreeSql.Ado.ExecuteDataSet(cmdText, parms);


        /// <summary>
        /// 执行SQL返回单个对象，Query&lt;object&gt;("select * from user where age > @age", new { age = 25 })<para></para>
        /// 提示：parms 参数还可以传 Dictionary&lt;string, object&gt;
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parms"></param>
        public IEnumerable<object> GetSingle(string cmdText, object parms = null)
        {
            var result = FreeSql.Ado.QuerySingle<object>(cmdText, parms);
            return JObject.FromObject(result);
        }

        /// <summary>
        /// 返回第一行第一列
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public object ExcuteScalar(string cmdText, object parms = null)
        {
            return FreeSql.Ado.ExecuteScalar(cmdText, parms);
        }

        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public object ExecuteNonQuery(string cmdText, object parms = null)
        {
            return FreeSql.Ado.ExecuteNonQuery(cmdText, parms);
        }


    }
}