using Fluid.Values;
using Fluid;
using Microsoft.AspNetCore.Mvc.Routing;
using OrchardCore.Liquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EasyOC.Scripting.Filters
{

    public class AbsoluteBaseUrlFilter : ILiquidFilter
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public AbsoluteBaseUrlFilter(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }
        public ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, LiquidTemplateContext context)
        {
            var relativePath = input.ToStringValue();

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return new ValueTask<FluidValue>(input);
            }

            var urlHelper = _urlHelperFactory.GetUrlHelper(context.ViewContext);

            var result = new StringValue(ToAbsoluteUrl(urlHelper, relativePath));
            return new ValueTask<FluidValue>(result);
        }
        public string GetBaseUrl(IUrlHelper url)
        {
            var _siteService = url.ActionContext.HttpContext.RequestServices.GetService<ISiteService>();
            var siteSettings = _siteService?.GetSiteSettingsAsync().GetAwaiter().GetResult();
            if (siteSettings != null)
            {
                return siteSettings.BaseUrl;
            }
            else
            {
                var request = url.ActionContext.HttpContext.Request;
                var scheme = request.Scheme;
                var host = request.Host.ToUriComponent();
                return $"{scheme}://{host}";
            }
        }

        public string ToAbsoluteUrl(IUrlHelper url, string virtualPath)
        {
            var baseUrl = GetBaseUrl(url);
            var path = url.Content(virtualPath);
            return $"{baseUrl}{path}";
        }
    }
}
