using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Transformalize",
    Author = "Dale Newman",
    Website = "https://github.com/dalenewman/OrchardCore.Transformalize",
    Version = "0.9.3",
    Description = "transformalize everything",
    Category = "ETL",
    Dependencies = new[]{
        "OrchardCore.Alias",
        "OrchardCore.ContentFields",
        "OrchardCore.Contents",
        "OrchardCore.Title",
        "OrchardCore.Users",
        "Etch.OrchardCore.ContentPermissions"
    }
)]