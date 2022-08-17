using EasyOC.Core;
using OrchardCore.Modules.Manifest;
using static EasyOC.ManifestConsts;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.OrchardCore.ContentExtentions",
    Description = "EasyOC.OrchardCore.ContentExtentions",
    Category = "Content Management",
    Dependencies = new[] { Constants.EasyOCCoreModuleId, "OrchardCore.ContentPreview", "OrchardCore.ContentFields", "OrchardCore.Contents" }
)]



