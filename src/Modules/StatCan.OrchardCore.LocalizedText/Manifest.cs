using OrchardCore.Modules.Manifest;
using StatCan.OrchardCore.LocalizedText;
using StatCan.OrchardCore.Manifest;

[assembly: Module(
    Name = "LocalizedText Part",
    Author = StatCanManifestConstants.DigitalInnovationTeam,
    Website = StatCanManifestConstants.DigitalInnovationWebsite,
    Version = StatCanManifestConstants.Version,
    Description = "Localization part that allows storing localized strings in a single content item",
    Category = "Content"
)]

[assembly: Feature(
    Id = Constants.Features.LocalizedText,
    Name = "LocalizedText Part",
    Description = "Part for managing localized strings inside a single ContentItem",
    Category = "Content",
    Dependencies = new[] { "OrchardCore.Localization" }
)]



