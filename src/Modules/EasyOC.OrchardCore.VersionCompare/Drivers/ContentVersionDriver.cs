using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.ViewModels;
using OrchardCore.Deployment;
using OrchardCore.DisplayManagement.Views;

namespace EasyOC.OrchardCore.VersionCompare.Drivers
{
    public class ContentVersionDriver : ContentDisplayDriver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public ContentVersionDriver(
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        public override IDisplayResult Display(ContentItem contentItem)
        {
            var context = _httpContextAccessor.HttpContext;

            return Shape("VersionCompare_SummaryAdmin__Button__Actions", new ContentItemViewModel(contentItem)).Location("SummaryAdmin", "ActionsMenu:20")
                    .RenderWhen(async () =>
                    {
                        var hasEditPermission = await _authorizationService.AuthorizeAsync(context.User, CommonPermissions.Export, contentItem);

                        if (hasEditPermission)
                        {
                            return true;
                        }

                        return false;
                    });
        }
    }
}



