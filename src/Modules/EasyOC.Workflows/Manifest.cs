using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.Workflows",
    Dependencies = new[] { "OrchardCore.Queries.Sql",
        "OrchardCore.Workflows",
        "OrchardCore.Workflows.Http"},
    Description = "EasyOC.Workflows",
    Category = "Workflow"
)]



