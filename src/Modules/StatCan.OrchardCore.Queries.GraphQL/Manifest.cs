using OrchardCore.Modules.Manifest;
using StatCan.OrchardCore.Manifest;

[assembly: Module(
    Name = "GraphQL Queries",
    Author = StatCanManifestConstants.DigitalInnovationTeam,
    Website = StatCanManifestConstants.DigitalInnovationWebsite,
    Version = StatCanManifestConstants.Version
)]

[assembly: Feature(
    Id = "StatCan.OrchardCore.Queries.GraphQL",
    Name = "GraphQL Queries",
    Description = "Introduces a way to create custom Queries using the GraphQL Language",
    Category = "GraphQL",
    Dependencies = new[]
    {
        "OrchardCore.Queries",
        "OrchardCore.Apis.GraphQL"
    }
)]



