﻿using OrchardCore.DisplayManagement.Manifest;
using OrchardCore.Modules.Manifest;

[assembly: Theme(
Name = "EasyOC Site Theme",
Author = "The EasyOC Team",
Version = "0.0.1",
// Tags = new[] { "Default" },
Dependencies = new[]
{
    "EasyOC.Core"
},
BaseTheme = "TheTheme",
Description = "EasyOC Site Theme"
)]
