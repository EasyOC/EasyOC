using OrchardCore.Modules.Manifest;
using static EasyOC.ManifestConsts;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.OrchardCore.Scripting",
    Dependencies = new[] { "OrchardCore.Queries" },
    Description = "Provide getSiteSettings('CustomSettingType'), Override executeQuery ",
    Category = "Infrastructure"
)]



