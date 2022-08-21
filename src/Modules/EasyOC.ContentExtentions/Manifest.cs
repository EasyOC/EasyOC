using EasyOC.Core;
using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.ContentExtentions",
    Description = "EasyOC.ContentExtentions",
    Category = "Content Management",
    Dependencies = new[] { Constants.EasyOCCoreModuleId, "OrchardCore.ContentPreview", "OrchardCore.ContentFields", "OrchardCore.Contents" }
)]



