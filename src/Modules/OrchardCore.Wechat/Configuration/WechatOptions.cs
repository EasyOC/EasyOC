using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace OrchardCore.Wechat.Configuration
{
    /// <summary>
    /// Configuration options for <see cref="WechatHandler"/>.
    /// </summary>
    public class WechatOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="WechatOptions"/>.
        /// </summary>
        public WechatOptions()
        {
            CallbackPath = new PathString("/signin-wechat");
            AuthorizationEndpoint = WechatDefaults.AuthorizationEndpoint;
            TokenEndpoint = WechatDefaults.TokenEndpoint;
            UserInformationEndpoint = WechatDefaults.UserInformationEndpoint;
            Scope.Add("snsapi_login");
            
            //ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);
            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "unionid");
            ClaimActions.MapJsonKey("openid", "openid");
            ClaimActions.MapJsonKey("unionid", "unionid");
            ClaimActions.MapJsonKey("name", "nickname");
            ClaimActions.MapJsonKey("sex", "sex");
            ClaimActions.MapJsonKey("language", "language");
            ClaimActions.MapJsonKey("city", "city");
            ClaimActions.MapJsonKey("province", "province");
            ClaimActions.MapJsonKey("country", "country");            
            ClaimActions.MapJsonKey("url", "headimgurl");
            ClaimActions.MapJsonKey("headimgurl", "headimgurl");
        }

        public bool EnableOfficialAccount { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }

    }
}
