using OrchardCore.DisplayManagement.Manifest;

[assembly: Theme(
    Name = "EasyOC Site Theme",
    Author = "The EasyOC Team",
    Version = "0.0.1", 
    BaseTheme = "TheTheme",
// Tags = new[] { "Default" },
    Dependencies = new[]
    {
        "EasyOC.Core"
    },
    Description = "EasyOC Site Theme",
    Tags = new[] { "Bootstrap", "Default" }
)]
