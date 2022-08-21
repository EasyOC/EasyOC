using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace EasyOC.OrchardCore.VueElementUI
{
    public class ElmentUIResourceManagementOptions : IConfigureOptions<ResourceManagementOptions>
    {
        private static global::OrchardCore.ResourceManagement.ResourceManifest _manifest;

        static ElmentUIResourceManagementOptions()
        {
            _manifest = new ResourceManifest();
            _manifest
                .DefineStyle("element-theme-chalk")
               .SetUrl("~/EasyOC.OrchardCore.VueElementUI/element-ui/theme-chalk/index.css")
               .SetCdn("https://unpkg.com/element-ui/lib/theme-chalk/index.css")
               .SetVersion("2.15.6");

            _manifest
                .DefineScript("elementUI")
                .SetUrl("~/EasyOC.OrchardCore.VueElementUI/element-ui/index.js")
                .SetCdn("https://unpkg.com/element-ui/lib/index.js")
                .SetVersion("2.15.6")
                .SetDependencies("vuejs", "element-theme-chalk")
                ;
        }

        public void Configure(ResourceManagementOptions options)
        {
            options.ResourceManifests.Add(_manifest);
        }
    }
}



