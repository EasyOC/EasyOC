using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.ContentExtensions.Scripting
{
    public class ContentMethodsProvider : IGlobalMethodProvider
    {
        public IEnumerable<GlobalMethod> GetMethods()
        {
            return new[]
            {
                    new GlobalMethod
                    {
                        Name = "contentItem",
                        Method = serviceProvider => (Func<string, object>)((contentItemId) =>
                        {
                            var contentManager = serviceProvider.GetRequiredService<IContentManager>();
                             return contentManager.GetAsync(contentItemId).GetAwaiter().GetResult();
                        })
                    },
                    new GlobalMethod
                    {
                        Name = "contentItemByVersion",
                        Method = serviceProvider => (Func<string, object>)(versionId =>
                        {
                            var contentManager = serviceProvider.GetRequiredService<IContentManager>();
                           return contentManager.GetVersionAsync(versionId).GetAwaiter().GetResult();
                        })
                    }
            };
        }
    }
}
