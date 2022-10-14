using EasyOC.Core;
using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Id = "EasyOC.GraphQL",
    Name = "EasyOC.GraphQL",
    Description = "EasyOC GraphQL Extensions",
    Dependencies = new[]
    {
        Constants.EasyOCCoreModuleId, "OrchardCore.ContentFields","OrchardCore.Media"
    },
    Category = "Content Management"
    )
]
