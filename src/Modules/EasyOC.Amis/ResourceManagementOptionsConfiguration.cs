using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace EasyOC.Amis
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {

        private const string AMISProjectPath = "~/EasyOC.Amis/";
        private const string AmisVersion = "1.9.1.28";
   

        ResourceManifest BuildManifest()
        {
            var manifest = new ResourceManifest();

            manifest
                .DefineScript("amis")
                .SetUrl(AMISProjectPath + "amis/sdk/sdk.js")
                .SetVersion(AmisVersion);
            manifest
                .DefineScript("axios")
                .SetUrl(AMISProjectPath + "Scripts/axios/dist/axios.mini.js", AMISProjectPath + "Scripts/axios/dist/axios.js")
                .SetVersion("0.27.2");
            manifest
                .DefineScript("amis-env")
                .SetDependencies("amis", "axios")
                .SetUrl(AMISProjectPath + "Scripts/AmisEnv.js");
            manifest
                .DefineScript("amis-ext")
                .SetDependencies("amis", "amis-env", "axios")
                .SetUrl(AMISProjectPath + "Scripts/AmisExt.js");

            manifest
                .DefineScript("amis-default")
                .SetDependencies("amis", "amis-env", "amis-ext", "axios");


            manifest
                .DefineStyle("amis")
                .SetUrl(AMISProjectPath + "amis/sdk/sdk.css")
                .SetVersion(AmisVersion);
            manifest
                .DefineStyle("amis-antd")
                .SetDependencies("amis")
                .SetUrl(AMISProjectPath + "amis/sdk/antd.css")
                .SetVersion(AmisVersion);
            manifest
                .DefineStyle("amis-cxd")
                .SetDependencies("amis")
                .SetUrl(AMISProjectPath + "amis/sdk/cxd.css")
                .SetVersion(AmisVersion);
            manifest
                .DefineStyle("amis-helper")
                .SetDependencies("amis")
                .SetUrl(AMISProjectPath + "amis/sdk/helper.css")
                .SetVersion(AmisVersion);
            manifest
                .DefineStyle("amis-iconfont")
                .SetDependencies("amis")
                .SetUrl(AMISProjectPath + "amis/sdk/iconfont.css")
                .SetVersion(AmisVersion);

            manifest
                .DefineStyle("amis-default")
                .SetDependencies("amis", "amis-cxd", "amis-helper", "amis-iconfont");

            return manifest;

        }

        public void Configure(ResourceManagementOptions options)
        {
            options.ResourceManifests.Add(BuildManifest()); 
 
        }
    }
}
