using EasyOC.Core;
using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "EasyOC.OrchardCore.ContentExtentions",
    Author = "The EasyOC Team",
    Website = "https://orchardcore.net",
    Version = "0.0.1",
    Description = "EasyOC.OrchardCore.ContentExtentions",
    Category = "Content Management",
    Dependencies = new[] { Constants.EasyOCCoreModuleId, "OrchardCore.ContentPreview", "OrchardCore.ContentFields", "OrchardCore.Contents" }
)]



