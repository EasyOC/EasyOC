using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "EasyOC.OrchardCore.Scripting",
    Author = "The EasyOC Team",
    Dependencies = new[] { "OrchardCore.Queries" },
    Description = "Provide getSiteSettings('CustomSettingType'), Override executeQuery ",
    Category = "Infrastructure"
)]



