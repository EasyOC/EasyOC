using OrchardCore.DisplayManagement.Manifest;
using OrchardCore.Modules.Manifest;

[assembly: Theme(
    Name = "EasyOCTheme",
    Author = "The EasyOC Team",
    Version = "0.0.1",
    Tags = new[] { ManifestConstants.AdminTag },
    Dependencies = new []{"EasyOC.Core","EasyOC.Workflows",
        "OrchardCore.ContentTypes",
        "StatCan.OrchardCore.DisplayHelpers",
        "StatCan.OrchardCore.Menu"},
    BaseTheme = "TheAdmin",
    Description = "EasyOCTheme"
)]
