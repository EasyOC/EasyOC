using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Entities;
using OrchardCore.Scripting;
using OrchardCore.Settings;
using System;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.Scripting.Providers
{
    public class EasyOCScriptExtendsProvider : IGlobalMethodProvider
    {
        public IEnumerable<GlobalMethod> GetMethods()
        {
            return new[] {
            new GlobalMethod{
              Name = "getSiteSettings",
                Method = serviceProvider => (Func<string,object>)((typeName) =>{
                        var _siteService= serviceProvider.GetService<ISiteService>();
                        var siteSettings = _siteService.GetSiteSettingsAsync().GetAwaiter().GetResult();
                        var settingContent = siteSettings.As<ContentItem>(typeName);
                        return JObject.FromObject(settingContent);

                    })
                }
            };
        }
    }
}



