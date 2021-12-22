using OrchardCore.Modules.Manifest;
using StatCan.OrchardCore.Manifest;

[assembly: Module(
    Name = "StatCan.OrchardCore.Scripting",
    Author = StatCanManifestConstants.DigitalInnovationTeam,
    Website = StatCanManifestConstants.DigitalInnovationWebsite,
    Version = StatCanManifestConstants.Version,
    Description = "Scripting methods",
    Category = "StatCan",
    Dependencies = new[] { "OrchardCore.Scripting" }
)]



