using EasyOC.OrchardCore.OpenApi.Indexs;
using EasyOC.OrchardCore.OpenApi.Model;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.DisplayManagement.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.OpenApi.Handlers
{
    public class VbenMenuHandler : ContentPartHandler<VbenMenuPart>
    {
        private readonly IFreeSql _freeSql;
        private readonly INotifier _notifier;
        private readonly IHtmlLocalizer H;
        public VbenMenuHandler(IFreeSql freeSql, INotifier notifier,
            IHtmlLocalizer<VbenMenuHandler> h)
        {
            _freeSql = freeSql;
            _notifier = notifier;
            H = h;
        }

        public override async Task PublishingAsync(PublishContentContext context, VbenMenuPart instance)
        {
            var validateResult = await CheckRoutePath(instance);
            if (validateResult.Any())
            {
                await _notifier.ErrorAsync(H["The RoutePath is duplicated with ：{0}", validateResult.JoinAsString(",")]);
                context.Cancel = true;
                return;
            }
            await base.PublishingAsync(context, instance);
        }

        public override async Task ValidatingAsync(ValidateContentContext context, VbenMenuPart instance)
        {
            var existsMenu = await CheckRoutePath(instance);
            if (existsMenu.Any())
            {
                context.Fail($"The RoutePath is duplicated with ：{existsMenu.FirstOrDefault()}", "MenuName");
                return;
            }
            await base.ValidatingAsync(context, instance);
        }
        private async Task<List<string>> CheckRoutePath(VbenMenuPart context)
        {
            var part = context.ContentItem.As<VbenMenuPart>();
            if (part.ContentItem.Latest && part.RoutePath is not null)
            {
                var parentId = part.ParentMenu.ContentItemIds.FirstOrDefault();
                var existsMenu = await _freeSql.Select<VbenMenuPartIndex>().Where(x => x.Published && x.Latest
                                  //limit in same parent
                                  && parentId == x.ParentMenu && part.RoutePath.Text.ToLower() == x.RoutePath
                                  && part.ContentItem.ContentItemId != x.ContentItemId)
                                    .ToListAsync(x => x.MenuName);

                return existsMenu;
            }
            else
            {
                return default;
            }
        }
    }
}
