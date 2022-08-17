using OrchardCore.Modules.Manifest;
using static EasyOC.ManifestConsts;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.OrchardCore.DynamicTypeIndex",
    Description = "EasyOC.OrchardCore.DynamicTypeIndex",
    Dependencies =new[] { "EasyOC.OrchardCore.ContentExtentions","EasyOC.OrchardCore.CSharpScript"},
    Category = "Content Management"
)]
