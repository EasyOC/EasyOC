using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace EasyOCVbenAdminTheme
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private static ResourceManifest _manifest;

        static ResourceManagementOptionsConfiguration()
        {
            _manifest = new ResourceManifest();

            // See https://github.com/marella/material-icons for usage
            //_manifest
            //    .DefineStyle("antdv")
            //    .SetCdn("https://fonts.googleapis.com/icon?family=Material+Icons|Material+Icons+Outlined|Material+Icons+Two+Tone|Material+Icons+Round|Material+Icons+Sharp")
            //    .SetUrl("~/EasyOCVbenAdminTheme/fonts/material-icons/material-icons.min.css", "~/EasyOCVbenAdminTheme/fonts/material-icons/material-icons.css")
            //    .SetVersion("1.11.0"); 
        }

        public void Configure(ResourceManagementOptions options)
        {
            options.ResourceManifests.Add(_manifest);
        }
    }
}
