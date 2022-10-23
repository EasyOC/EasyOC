using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Entities;
using OrchardCore.Scripting;
using OrchardCore.Settings;
using OrchardCore.Users;
using System;
using System.Collections.Generic;

namespace EasyOC.Scripting.Providers
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
                },
            new GlobalMethod{Name = "getCurrentUser",
                Method = serviceProvider =>(()=>GetUser(serviceProvider)) },
            //new GlobalMethod{Name = "getUserManager", Method = serviceProvider =>GetUserManager }

            };
        }

        //private UserManager<IUser> GetUserManager(IServiceProvider serviceProvider)
        //{
        //    var userManager = serviceProvider.GetRequiredService<UserManager<IUser>>();
        //    return userManager;
        //}
        private object GetUser(IServiceProvider serviceProvider)
        {
            var http = serviceProvider.GetService<IHttpContextAccessor>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IUser>>();
            var httpUser = http?.HttpContext?.User;
            if (httpUser != null)
            {
                var user = userManager.GetUserAsync(httpUser).GetAwaiter().GetResult();
                var jobjUser = JObject.FromObject(user);
                jobjUser.Remove("SecurityStamp");
                jobjUser.Remove("PasswordHash");
            }
            return null;
        }
    }
}



