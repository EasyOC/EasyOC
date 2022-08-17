using OrchardCore.Modules.Manifest;
using System;
using static EasyOC.ManifestConsts;

[assembly: Module(
    Author = Author,
    Website = Website,
    Version = CurrentVersion,
    Name = "EasyOC.Lucene",
    Description = "EasyOC.Lucene",
    Dependencies = new[]
    {
        "OrchardCore.Lucene"
    },
    Category = "Content Management"
)]
