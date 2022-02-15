using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Threading.Tasks;

namespace JZSoft.OrchardCore.Amis
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
                  .Add(S["Amis Editor"], S["Amis Editor"].PrefixPosition("4"),
                                action => action
                                .Action("AmisEditor", "Admin", new { area = "JZSoft.OrchardCore.Amis" })
                                .Permission(Permissions.Amis_Editor)
                                .LocalNav())
                            )
                    );
            return Task.CompletedTask;
        }
    }
}
