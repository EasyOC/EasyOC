using EasyOC.Core;
using OrchardCore.Modules.Manifest;
using static EasyOC.ManifestConsts;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Id = "EasyOC.GraphQL",
    Name = "EasyOC.GraphQL",
    Description = "EasyOC GraphQL Extensions",
    Dependencies = new[]
    {
        Constants.EasyOCCoreModuleId, "OrchardCore.Apis.GraphQL","OrchardCore.ContentFields","OrchardCore.Media"
    },
    Category = "Content Management"
    )
]
