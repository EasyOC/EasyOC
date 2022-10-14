using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.RDBMS",
    Dependencies = new[]
    {
        "OrchardCore.Contents", "EasyOC.Excel", "EasyOC.VueElementUI","OrchardCore.Queries"
    },
    Description = "EasyOC.RDBMS",
    Category = "Content Management"
)]
