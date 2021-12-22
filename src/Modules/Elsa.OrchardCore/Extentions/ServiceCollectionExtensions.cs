using Elsa.Options;
using Elsa.Persistence.YesSql;
using Elsa.Persistence.YesSql.Data;
using Elsa.Persistence.YesSql.Indexes;
using Elsa.Persistence.YesSql.Mapping;
using Elsa.Persistence.YesSql.Services;
using Elsa.Persistence.YesSql.Stores;
using Elsa.Runtime;
using Microsoft.Extensions.DependencyInjection;
using System;
using YesSql;

namespace Elsa.OrchardCore
{
    public static class ServiceCollectionExtensions
    {
        public static ElsaOptionsBuilder UseOrchardCorePersistence(this ElsaOptionsBuilder elsa)
        {

            elsa.Services.AddScoped<YesSqlWorkflowDefinitionStore>().AddScoped<YesSqlWorkflowInstanceStore>().AddScoped<YesSqlWorkflowExecutionLogStore>()
                .AddScoped<YesSqlBookmarkStore>()
                //.AddSingleton((IServiceProvider sp) => CreateStore(sp)) //Use the already existing OrchardCore Store
                .AddSingleton<ISessionProvider, SessionProvider>()
                .AddScoped(new Func<IServiceProvider, ISession>(CreateSession))
                .AddScoped<IDataMigrationManager, DataMigrationManager>()
                .AddStartupTask<DatabaseInitializer>()
                .AddStartupTask<RunMigrations>()
                .AddDataMigration<Migrations>()
                .AddAutoMapperProfile<AutoMapperProfile>()
                .AddIndexProvider<WorkflowDefinitionIndexProvider>()
                .AddIndexProvider<WorkflowInstanceIndexProvider>()
                .AddIndexProvider<WorkflowExecutionLogRecordIndexProvider>()
                .AddIndexProvider<BookmarkIndexProvider>();
            return elsa.UseWorkflowDefinitionStore((IServiceProvider sp)
                    => sp.GetRequiredService<YesSqlWorkflowDefinitionStore>()).UseWorkflowInstanceStore((IServiceProvider sp)
                        => sp.GetRequiredService<YesSqlWorkflowInstanceStore>()).UseWorkflowExecutionLogStore((IServiceProvider sp)
                            => sp.GetRequiredService<YesSqlWorkflowExecutionLogStore>());



        }

        private static IStore CreateStore(IServiceProvider serviceProvider)
        {
            var result = serviceProvider.GetService<IStore>();
            //IEnumerable<IIndexProvider> services = serviceProvider.GetServices<IIndexProvider>();
            //result.RegisterIndexes(services);
            return result;
            //Configuration configuration = new Configuration
            //{
            //    ContentSerializer = new CustomJsonContentSerializer()
            //};
            //configure(serviceProvider, configuration);
            //IStore result = StoreFactory.CreateAndInitializeAsync(configuration).GetAwaiter().GetResult();
            //IEnumerable<IIndexProvider> services = serviceProvider.GetServices<IIndexProvider>();
            //result.RegisterIndexes(services);
            //return result;
        }
        private static ISession CreateSession(IServiceProvider serviceProvider)
        {
            ISessionProvider requiredService = serviceProvider.GetRequiredService<ISessionProvider>();
            return requiredService.CreateSession();
        }

    }
}
