using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Navigation;
using OrchardCore.Queries;
using OrchardCore.Security.Permissions;
using StatCan.OrchardCore.Queries.GraphQL.Controllers;
using StatCan.OrchardCore.Queries.GraphQL.Drivers;
using StatCan.OrchardCore.Queries.GraphQL.Services;
using System;

namespace StatCan.OrchardCore.Queries.GraphQL
{
    /// <summary>
    /// These services are registered on the tenant service collection
    /// </summary>
    [Feature("StatCan.OrchardCore.Queries.GraphQL")]
    public class Startup : StartupBase
    {
        private readonly AdminOptions _adminOptions;

        public Startup(IOptions<AdminOptions> adminOptions)
        {
            _adminOptions = adminOptions.Value;
        }
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<IDisplayDriver<Query>, GraphQLQueryDisplayDriver>();
            services.AddScoped<IQuerySource, GraphQLQuerySource>();
            services.AddScoped<IGraphQLQueryService, GraphQLQueryService>();
            services.AddScoped<INavigationProvider, AdminMenu>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            var adminControllerName = typeof(AdminController).ControllerName();

            routes.MapAreaControllerRoute(
                name: "QueriesRunGraphQL",
                areaName: "StatCan.OrchardCore.Queries.GraphQL",
                pattern: _adminOptions.AdminUrlPrefix + "/Queries/GraphQL/Query",
                defaults: new { controller = adminControllerName, action = nameof(AdminController.Query) }
            );
        }
    }
}



