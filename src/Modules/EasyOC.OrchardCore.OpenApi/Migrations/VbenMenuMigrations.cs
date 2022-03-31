using EasyOC.OrchardCore.OpenApi.Indexs;
using Microsoft.Extensions.FileProviders;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Data.Migration;
using OrchardCore.Deployment.Services;
using OrchardCore.Recipes.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using YesSql.Sql;

namespace EasyOC.OrchardCore.OpenApi.Migrations
{
    public class VbenMenuMigrations : DataMigration
    {
        private readonly IRecipeMigrator _recipeMigrator;
        private readonly IFreeSql _freeSql;
        public VbenMenuMigrations(IRecipeMigrator recipeMigrator, IFreeSql freeSql)
        {
            _recipeMigrator = recipeMigrator;
            _freeSql = freeSql;
        }

        public async Task<int> CreateAsync()
        {
            //Use FreeSql
            //create or update table
            // auto create or update index
            _freeSql.CodeFirst.SyncStructure<VbenMenuPartIndex>();
            SchemaBuilder.CreateForeignKey<VbenMenuPartIndex>();

            return await Task.FromResult(1);
        }
    }
}
