using Flurl;
using TransformalizeModule.Services.Contracts;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace TransformalizeModule.Services {
   public class LinkService : ILinkService {
      private readonly IHttpContextAccessor _contextAccessor;

      public LinkService(IHttpContextAccessor contextAccessor) {
         _contextAccessor = contextAccessor;
      }

      public HtmlString Create(string contentItemId, string actionUrl, bool everything) {

         var url = RemoveNoiseFromUrl(_contextAccessor.HttpContext.Request.GetDisplayUrl());
         url.Path = actionUrl;

         if (everything) {
            url.SetQueryParam("page", 0);
         } else {
            if (_contextAccessor.HttpContext.Request.Query.ContainsKey("size")) {
               var qSize = _contextAccessor.HttpContext.Request.Query["size"].ToString() ?? string.Empty;
               if (!int.TryParse(qSize, out _)) {
                  url.SetQueryParam("size", 20);
               }
            }
            if (_contextAccessor.HttpContext.Request.Query.ContainsKey("page")) {
               var qPage = _contextAccessor.HttpContext.Request.Query["page"].ToString() ?? string.Empty;
               if (int.TryParse(qPage, out int page)) {
                  if(page == 0) {
                     url.RemoveQueryParam("page");
                  }
               } else {
                  url.RemoveQueryParam("page");
               }
            }
         }

         return new HtmlString(url);

      }

      private static Url RemoveNoiseFromUrl(Url url) {

         var stars = (from param in url.QueryParams where param.Value.Equals("*") || param.Value.Equals("") select param.Name).ToList();
         foreach (var star in stars) {
            url.QueryParams.Remove(star);
         }

         return url;
      }
   }
}
