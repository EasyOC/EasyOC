using EasyOC.OrchardCore.OpenApi.Indexs;
using Microsoft.Extensions.FileProviders;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Data.Migration;
using OrchardCore.Deployment.Services;
using OrchardCore.Recipes.Services;
using System.IO;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Migrations
{
    public class UserProfileMigrations : DataMigration
    {
        private readonly IDeploymentManager _deploymentManager;
        private readonly IRecipeMigrator _recipeMigrator;
        public UserProfileMigrations(IDeploymentManager deploymentManager, IRecipeMigrator recipeMigrator)
        {
            _deploymentManager = deploymentManager;
            _recipeMigrator = recipeMigrator;
        }

        public int Create()
        {
            // 使用FreeSql同步实体类型到数据库
            SchemaBuilder.CreateMapIndexTable<UserProfileIndex>();
            return 1;
        }

        public int UpdateFrom1()
        {
            Create();
            return 2;
        }

        public async Task<int> UpdateFrom2Async()
        {
            var str = await _recipeMigrator.ExecuteAsync("UserProfiles.json", this);
            System.Console.WriteLine(str);
            return 3;
        }
      
    }
}
