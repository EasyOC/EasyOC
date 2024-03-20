using OrchardCore.DisplayManagement.Manifest;
using OrchardCore.Modules.Manifest;

[assembly: Theme(
    Name = "The VbenAdmin Theme",
    Author = "The EasyOC Team",
    Dependencies = new[] { "OrchardCore.Themes" },
    Version = "0.0.1",
    Description = "The VbenAdmin Theme.",
    BaseTheme = "TheAdmin",
    Tags = new[] { ManifestConstants.AdminTag }

)]
