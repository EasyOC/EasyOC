using OrchardCore.Wechat;
using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Wechat",
    Author = " Pim Hwang",
    Website = "http://www.coming.com.cn",
    Version = "0.0.1",
    Description = " ",
    Category = "Wechat"
)]

[assembly: Feature(
    Id = WechatConstants.Features.WechatAuthentication,
    Name = "Wechat Authentication",
    Category = "Wechat",
    Description = "Authenticates users with their Wechat Account."
)]