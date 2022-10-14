using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;
[assembly: Module(
    Name = "EasyOC.AppCenter",
    Author = Author,
    Website =  Website,
    Version = CurrentVersion,
    Description = "EasyOC.AppCenter",
    Category = "Content Management",
    DefaultTenantOnly = true,
    IsAlwaysEnabled = true
)]
