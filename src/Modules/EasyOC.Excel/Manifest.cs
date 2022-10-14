using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.Excel",
    Description = "EasyOC.Excel",
    Category = "Content Management",
    Dependencies = new[] { EasyOC.Core.Constants.EasyOCCoreModuleId,
        "OrchardCore.Apis.GraphQL",
        "OrchardCore.ContentFields",
        "EasyOC.GraphQL",
        "EasyOC.VueElementUI"}

)]



