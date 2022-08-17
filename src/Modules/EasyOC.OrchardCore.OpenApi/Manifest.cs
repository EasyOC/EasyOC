using OrchardCore.Modules.Manifest;
using static EasyOC.ManifestConsts;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.OrchardCore.OpenApi",
    Description = "EasyOC.OrchardCore.OpenApi",
    Dependencies = new[] { "EasyOC.Core","OrchardCore.Recipes", "EasyOC.OrchardCore.DynamicTypeIndex" },
    Category = "Content Management"
)]
