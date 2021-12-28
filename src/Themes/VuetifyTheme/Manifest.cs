using OrchardCore.DisplayManagement.Manifest;

[assembly: Theme(
    Name = "VuetifyTheme",
    Author = DigitalInnovationTeam,
    Website = DigitalInnovationWebsite,
    Version = "1.0.",
    Description = "Vuetify platform theme",
    Dependencies = new[] {
        "OrchardCore.ContentTypes",
        "StatCan.OrchardCore.DisplayHelpers",
        "StatCan.OrchardCore.Menu"
    }
)]



