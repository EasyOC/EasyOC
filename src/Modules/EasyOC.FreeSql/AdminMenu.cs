using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using OrchardCore.Queries.Sql;
using System;
using System.Threading.Tasks;

namespace EasyOC.FreeSql
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            S = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            builder
                .Add(S["Search"], search => search
                    .Add(S["Queries"], S["Queries"].PrefixPosition(), queries => queries
                        .Add(S["Run FreeSQL Query"], S["Run FreeSQL Query"].PrefixPosition(), sql => sql
                             .Action("Query", "Admin", new { area = "EasyOC.FreeSql" })
                             .Permission(Permissions.ManageSqlQueries)
                             .LocalNav())));

            return Task.CompletedTask;
        }
    }
}
