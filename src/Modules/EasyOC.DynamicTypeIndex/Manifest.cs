using OrchardCore.Modules.Manifest;
using static EasyOC.Constants.ManifestConstants;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.DynamicTypeIndex",
    Description = "EasyOC.DynamicTypeIndex",
    Dependencies =new[] { "EasyOC.ContentExtentions","EasyOC.CSharpScript"},
    Category = "Content Management"
)]
