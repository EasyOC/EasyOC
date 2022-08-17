using OrchardCore.Modules.Manifest;
using static EasyOC.ManifestConsts;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.GraphQL",
    Description = "EasyOC GraphQL Extensions",
    Dependencies = new[] { "EasyOC.Core","OrchardCore.Apis.GraphQL" },
    Category = "Content Management"
)]
