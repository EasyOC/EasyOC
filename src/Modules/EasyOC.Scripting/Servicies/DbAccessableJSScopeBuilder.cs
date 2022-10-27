using EasyOC.RDBMS;
using EasyOC.RDBMS.Models;
using EasyOC.RDBMS.Services;
using Jint;
using Microsoft.Extensions.Logging;
using OrchardCore.Scripting;
using OrchardCore.Scripting.JavaScript;
using System;
using System.Threading.Tasks;

namespace EasyOC.Scripting.Servicies
{
    public class DbAccessableJSScopeBuilder : IDbAccessableJSScopeBuilder
    {
        private readonly IScriptingManager _scriptingManager;
        private readonly IRDBMSAppService _rDbmsAppService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public DbAccessableJSScopeBuilder(IScriptingManager scriptingManager, IRDBMSAppService rDbmsAppService,
            ILogger<DbAccessableJSScopeBuilder> logger,
            IServiceProvider serviceProvider)
        {
            _scriptingManager = scriptingManager;
            _rDbmsAppService = rDbmsAppService;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        public async Task<JavaScriptScope> CreateScopeAsync()
        {
            var scope = _scriptingManager.CreateScope("js", _serviceProvider)
                      as JavaScriptScope;
            var engine = scope.Engine;


            // 注册数据库访问对象
            var connections = await _rDbmsAppService.GetAllDbConnection();

            foreach (var item in connections)
            {
                engine.SetValue(item.ConfigName, new ExternalDbProvider(_serviceProvider, new ExternalDbConfig
                {
                    Name = item.ConfigName,
                    ConnectionConfigId = item.ConfigId
                }, _logger));
            }
            engine.SetValue(RDBMS.Constants.ShellDbName, new ExternalDbProvider(_serviceProvider, new ExternalDbConfig
            {
                Name = RDBMS.Constants.ShellDbName,
                ConnectionConfigId = RDBMS.Constants.ShellDbName
            }, _logger));
            return scope;
        }
    }
}
