using OrchardCore.DisplayManagement.Manifest;
using OrchardCore.Modules.Manifest;

[assembly: Theme(
    Name = "EasyOCTheme",
    Author = "The EasyOC Team",
    Version = "0.0.1",
    Tags = new[] { ManifestConstants.AdminTag },
    Dependencies = new []{"EasyOC.OrchardCore.WorkflowPlus"},
    BaseTheme = "TheAdmin",
    Description = "EasyOCTheme"
)]
