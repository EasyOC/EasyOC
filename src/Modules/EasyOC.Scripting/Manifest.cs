using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.Scripting",
    Dependencies = new[] { "OrchardCore.Queries","EasyOC.RDBMS","EasyOC.Amis" },
    Description = "Provide getSiteSettings('CustomSettingType'), Override executeQuery ",
    Category = "Infrastructure"
)]



