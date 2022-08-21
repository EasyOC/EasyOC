using OrchardCore.Modules.Manifest;
using static EasyOC.ManifestConsts;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.OrchardCore.Excel",
    Description = "EasyOC.OrchardCore.Excel",
    Category = "Content Management",
    Dependencies = new[] { EasyOC.Core.Constants.EasyOCCoreModuleId,
        "OrchardCore.Apis.GraphQL",
        "OrchardCore.ContentFields",
        "EasyOC.OrchardCore.VueElementUI"}

)]



