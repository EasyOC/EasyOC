using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using OrchardCore.RelationDb;
using System;
using System.Threading.Tasks;

namespace EasyOC.RDBMS
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
            if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }
            builder.Add(S["Content"], content => content
              .Add(S["Content Definition"], S["Content Definition"].PrefixPosition("9"), contentDefinition => contentDefinition
                  .Add(S["RDBMS"], S["RDBMS"].PrefixPosition("3"),
                                queries => queries
                                .Action("CreateOrEdit", "Admin", new { area = "EasyOC.RDBMS" })
                                .Permission(Permissions.ManageRelationalDbTypes)
                                .LocalNav())
                            )
                    );
            return Task.CompletedTask;
        }
    }
}



