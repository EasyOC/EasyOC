using EasyOC.OrchardCore.OpenApi.Indexes;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Recipes.Services;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Migrations
{
    public class VbenMenuMigrations : DataMigration
    {

        public async Task<int> CreateAsync()
        {
            var _freeSql = ShellScope.Services.GetRequiredService<IFreeSql>();
            //Use FreeSql
            //create or update table , auto create or update index
            _freeSql.CodeFirst.SyncStructure<VbenMenuPartIndex>();
            SchemaBuilder.CreateForeignKey<VbenMenuPartIndex>();

            return await Task.FromResult(1);
        }
    }
}
