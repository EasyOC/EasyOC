using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Description = "EasyOC.FreeSql",
    Dependencies = new[]
    {
        "EasyOC.DynamicTypeIndex"
    },
    Name = "EasyOC.FreeSql",
    Category = "Content Management"
)]
