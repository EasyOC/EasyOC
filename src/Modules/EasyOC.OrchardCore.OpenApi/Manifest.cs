using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "EasyOC.OrchardCore.OpenApi",
    Author = "The Orchard Core Team",
    Website = "https://orchardcore.net",
    Version = "0.0.1",
    Description = "EasyOC.OrchardCore.OpenApi",
    Dependencies = new[] { "EasyOC.Core","OrchardCore.Recipes", "EasyOC.OrchardCore.DynamicTypeIndex" },
    Category = "Content Management"
)]
