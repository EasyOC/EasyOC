using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.OpenApi",
    Description = "EasyOC.OpenApi",
    Dependencies = new[] { "EasyOC.Core","OrchardCore.Recipes", "EasyOC.DynamicTypeIndex","EasyOC.GraphQL" },
    Category = "Content Management"
)]
