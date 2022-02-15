using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Modules;
using OrchardCore.Navigation;

namespace OrchardCore.Wechat
{

    [Feature(WechatConstants.Features.WechatAuthentication)]
    public class AdminMenuWechatLogin : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public AdminMenuWechatLogin(IStringLocalizer<AdminMenuWechatLogin> localizer)
        {
            S = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                builder.Add(S["Security"], security => security
                        .Add(S["Authentication"], authentication => authentication
                        .Add(S["Wechat"], S["Wechat"].PrefixPosition(), settings => settings
                        .AddClass("wechat").Id("wechat")
                            .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = WechatConstants.Features.WechatAuthentication })
                            .Permission(Permissions.ManageWechatAuthentication)
                            .LocalNav())
                    ));

            }

            return Task.CompletedTask;
        }
    }
}
