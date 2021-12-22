using OrchardCore.Modules.Manifest;
using StatCan.OrchardCore.GCCollab;
using StatCan.OrchardCore.Manifest;

[assembly: Module(
    Name = "GCCollab",
    Author = StatCanManifestConstants.DigitalInnovationTeam,
    Website = StatCanManifestConstants.DigitalInnovationWebsite,
    Version = StatCanManifestConstants.Version,
    Category = "GCCollab"
)]

[assembly: Feature(
    Id = GCCollabConstants.Features.GCCollabAuthentication,
    Name = "GCCollab Authentication",
    Category = "GCCollab",
    Description = "Authenticates users with their GCCollab Account."
)]



