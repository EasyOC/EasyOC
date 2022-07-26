using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "EasyOC.OrchardCore.Excel",
    Author = "The EasyOC Team",
    Website = "https://orchardcore.net",
    Version = "0.0.1",
    Description = "EasyOC.OrchardCore.Excel",
    Category = "Content Management",
    Dependencies = new[] { EasyOC.Core.Constants.EasyOCCoreModuleId,
        "OrchardCore.Apis.GraphQL",
        "OrchardCore.ContentFields",
        "EasyOC.OrchardCore.VueElementUI"}

)]



