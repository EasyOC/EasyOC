using Microsoft.AspNetCore.Http;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;

namespace StatCan.OrchardCore.Scripting
{
    public class HttpGlobalMethodsProvider : IGlobalMethodProvider
    {
        private readonly GlobalMethod _redirect;

        public HttpGlobalMethodsProvider(IHttpContextAccessor httpContextAccessor)
        {
            _redirect = new GlobalMethod
            {
                Name = "httpRedirect",
                Method = serviceProvider => (Action<String>)((url) =>
                {
                    if (url.StartsWith("~/"))
                    {
                        url = httpContextAccessor.HttpContext.Request.PathBase + url[1..];
                    }
                    if (url?.Length == 0)
                    {
                        url = httpContextAccessor.HttpContext.Request.PathBase;
                    }
                    httpContextAccessor.HttpContext.Response.Redirect(url);
                }
                )
            };
        }

        public IEnumerable<GlobalMethod> GetMethods()
        {
            return new[] { _redirect };
        }
    }
}



