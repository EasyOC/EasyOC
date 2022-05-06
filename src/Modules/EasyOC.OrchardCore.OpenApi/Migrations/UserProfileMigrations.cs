using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Recipes.Services;
using OrchardCore.Users.Handlers;
using OrchardCore.Users.Indexes;
using OrchardCore.Users.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.OpenApi.Migrations
{
    public class UserProfileMigrations : DataMigration
    {
        private readonly IRecipeMigrator _recipeMigrator;
        private readonly ILogger _logger;
        private readonly IEnumerable<IUserEventHandler> Handlers;
        private readonly ISession _session;

        public UserProfileMigrations(IRecipeMigrator recipeMigrator,IEnumerable<IUserEventHandler> handlers,
            ILogger<UserProfileMigrations> logger, ISession session)
        {
            _recipeMigrator = recipeMigrator;
            Handlers = handlers;
            _logger = logger;
            _session = session;
        }

        public async Task<int> CreateAsync()
        {
            //查找尚未创建索引的数据
            var users = await _session.Query<User, UserIndex>().ListAsync();
            foreach (var user in users)
            {
                var context = new UserCreateContext(user);
                //触发事件：为已存在用户创建索引
                await Handlers.InvokeAsync((handler, context) => handler.CreatedAsync(context), context, _logger);
            } 
            return 1;
        }
    }
}
