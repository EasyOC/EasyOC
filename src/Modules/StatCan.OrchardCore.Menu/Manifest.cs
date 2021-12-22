using OrchardCore.Modules.Manifest;
using StatCan.OrchardCore.Manifest;

[assembly: Module(
    Name = "StatCan.OrchardCore.Menu",
    Author = StatCanManifestConstants.DigitalInnovationTeam,
    Website = StatCanManifestConstants.DigitalInnovationWebsite,
    Version = StatCanManifestConstants.Version,
    Description = "Menu overrides",
    Category = "StatCan",
    Dependencies = new[] { "OrchardCore.Menu" }
)]



