using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "EasyOC.OrchardCore.WorkflowPlus",
    Author = "The EasyOC Team",
    Website = "https://orchardcore.net",
    Version = "0.0.1",
    Dependencies = new[] { "OrchardCore.Queries.Sql",
        "OrchardCore.Workflows",
        "OrchardCore.Workflows.Http"},
    Description = "EasyOC.OrchardCore.WorkflowPlus",
    Category = "Workflow"
)]



