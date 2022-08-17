using OrchardCore.Modules.Manifest;
using static EasyOC.ManifestConsts;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.OrchardCore.RDBMS",
    Dependencies = new[]
    {
        "OrchardCore.Contents", "EasyOC.OrchardCore.Excel", "EasyOC.OrchardCore.VueElementUI"
    },
    Description = "EasyOC.OrchardCore.RDBMS",
    Category = "Content Management"
)]
