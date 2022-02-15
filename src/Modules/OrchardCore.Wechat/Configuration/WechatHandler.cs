using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;



namespace OrchardCore.Wechat.Configuration
{
    public class WechatHandler : OAuthHandler<WechatOptions>
    {
        private const string CorrelationProperty = ".xsrf";
        private const string CorrelationMarker = "N";
        private static IMemoryCache _properties = new MemoryCache(new MemoryCacheOptions());
       
        public WechatHandler(IOptionsMonitor<WechatOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        /// <summary>
        /// This code was copied from the aspnetcore repository . 
        /// https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/Core/src/RemoteAuthenticationHandler.cs#L241
        /// </summary>
        protected override bool ValidateCorrelationId(AuthenticationProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (!properties.Items.TryGetValue(CorrelationProperty, out string correlationId))
            {
                //Logger.CorrelationPropertyNotFound(Options.CorrelationCookie.Name);
                return false;
            }

            properties.Items.Remove(CorrelationProperty);

            var cookieName = Options.CorrelationCookie.Name + Scheme.Name + "." + correlationId;

            var correlationCookie = Request.Cookies[cookieName];
            if (string.IsNullOrEmpty(correlationCookie))
            {
                //Logger.CorrelationCookieNotFound(cookieName);
                return false;
            }

            var cookieOptions = Options.CorrelationCookie.Build(Context, Clock.UtcNow);

            Response.Cookies.Delete(cookieName, cookieOptions);

            if (!string.Equals(correlationCookie, CorrelationMarker, StringComparison.Ordinal))
            {
                //Logger.UnexpectedCorrelationCookieValue(cookieName, correlationCookie);
                return false;
            }

            return true;
        }

        /// <summary>
        /// This code was copied from the aspnetcore repository . 
        /// https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/OAuth/src/OAuthHandler.cs#L55
        /// </summary>
        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var query = Request.Query;

            string state = query["state"];
            AuthenticationProperties properties = Options.StateDataFormat.Unprotect(state);

            if (properties == null && IsMicroMessenger(Request))
            {
                if (_properties.TryGetValue<AuthenticationProperties>(state, out properties))
                {
                    _properties.Remove(state);
                }
            }

            if (properties == null)
            {
                return HandleRequestResult.Fail("The oauth state was missing or invalid.");
            }

            // OAuth2 10.12 CSRF
            if (!ValidateCorrelationId(properties))
            {
                return HandleRequestResult.Fail("Correlation failed.", properties);
            }

            var error = query["error"];
            if (!StringValues.IsNullOrEmpty(error))
            {
                // Note: access_denied errors are special protocol errors indicating the user didn't
                // approve the authorization demand requested by the remote authorization server.
                // Since it's a frequent scenario (that is not caused by incorrect configuration),
                // denied errors are handled differently using HandleAccessDeniedErrorAsync().
                // Visit https://tools.ietf.org/html/rfc6749#section-4.1.2.1 for more information.
                var errorDescription = query["error_description"];
                var errorUri = query["error_uri"];
                if (StringValues.Equals(error, "access_denied"))
                {
                    var result = await HandleAccessDeniedErrorAsync(properties);
                    if (!result.None)
                    {
                        return result;
                    }
                    var deniedEx = new Exception("Access was denied by the resource owner or by the remote server.");
                    deniedEx.Data["error"] = error.ToString();
                    deniedEx.Data["error_description"] = errorDescription.ToString();
                    deniedEx.Data["error_uri"] = errorUri.ToString();

                    return HandleRequestResult.Fail(deniedEx, properties);
                }

                var failureMessage = new StringBuilder();
                failureMessage.Append(error);
                if (!StringValues.IsNullOrEmpty(errorDescription))
                {
                    failureMessage.Append(";Description=").Append(errorDescription);
                }
                if (!StringValues.IsNullOrEmpty(errorUri))
                {
                    failureMessage.Append(";Uri=").Append(errorUri);
                }

                var ex = new Exception(failureMessage.ToString());
                ex.Data["error"] = error.ToString();
                ex.Data["error_description"] = errorDescription.ToString();
                ex.Data["error_uri"] = errorUri.ToString();

                return HandleRequestResult.Fail(ex, properties);
            }

            var code = query["code"];

            if (StringValues.IsNullOrEmpty(code))
            {
                return HandleRequestResult.Fail("Code was not found.", properties);
            }

            var codeExchangeContext = new OAuthCodeExchangeContext(properties, code, BuildRedirectUri(Options.CallbackPath));
            using var tokens = await ExchangeCodeAsync(codeExchangeContext);

            if (tokens.Error != null)
            {
                return HandleRequestResult.Fail(tokens.Error, properties);
            }

            if (string.IsNullOrEmpty(tokens.AccessToken))
            {
                return HandleRequestResult.Fail("Failed to retrieve access token.", properties);
            }

            var identity = new ClaimsIdentity(ClaimsIssuer);

            if (Options.SaveTokens)
            {
                var authTokens = new List<AuthenticationToken>();

                authTokens.Add(new AuthenticationToken { Name = "access_token", Value = tokens.AccessToken });
                if (!string.IsNullOrEmpty(tokens.RefreshToken))
                {
                    authTokens.Add(new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken });
                }

                if (!string.IsNullOrEmpty(tokens.TokenType))
                {
                    authTokens.Add(new AuthenticationToken { Name = "token_type", Value = tokens.TokenType });
                }

                if (!string.IsNullOrEmpty(tokens.ExpiresIn))
                {
                    int value;
                    if (int.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                    {
                        // https://www.w3.org/TR/xmlschema-2/#dateTime
                        // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                        var expiresAt = Clock.UtcNow + TimeSpan.FromSeconds(value);
                        authTokens.Add(new AuthenticationToken
                        {
                            Name = "expires_at",
                            Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                        });
                    }
                }

                properties.StoreTokens(authTokens);
            }

            var ticket = await CreateTicketAsync(identity, properties, tokens);
            if (ticket != null)
            {
                return HandleRequestResult.Success(ticket);
            }
            else
            {
                return HandleRequestResult.Fail("Failed to retrieve user information from remote server.", properties);
            }
        }

        /// <summary>
        /// This code was copied from the aspnetcore repository . 
        /// https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/OAuth/src/OAuthHandler.cs#L291
        /// </summary>
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            var isMicroMessenger = IsMicroMessenger(Request);
            var authorizationEndpoint = isMicroMessenger ? WechatDefaults.AuthorizationEndpointOauth : Options.AuthorizationEndpoint;
            var scopeParameter = properties.GetParameter<ICollection<string>>(OAuthChallengeProperties.ScopeKey);
            var scope = scopeParameter != null ? FormatScope(scopeParameter) : FormatScope();
            var callbackUrl = string.Empty;

            var parameters = new Dictionary<string, string>
            {
                { "appid", isMicroMessenger ? Options.AppId : Options.ClientId },
                { "scope", isMicroMessenger ? "snsapi_userinfo": scope },
                { "response_type", "code" },
                { "redirect_uri", string.IsNullOrEmpty(callbackUrl) ? redirectUri : callbackUrl + redirectUri }
            };

            if (Options.UsePkce)
            {
                var bytes = new byte[32];
                RandomNumberGenerator.Fill(bytes);
                var codeVerifier = Microsoft.AspNetCore.WebUtilities.Base64UrlTextEncoder.Encode(bytes);

                // Store this for use during the code redemption.
                properties.Items.Add(OAuthConstants.CodeVerifierKey, codeVerifier);

                using var sha256 = SHA256.Create();
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                var codeChallenge = WebEncoders.Base64UrlEncode(challengeBytes);

                parameters[OAuthConstants.CodeChallengeKey] = codeChallenge;
                parameters[OAuthConstants.CodeChallengeMethodKey] = OAuthConstants.CodeChallengeMethodS256;
            }

            var state = Options.StateDataFormat.Protect(properties);
            
            if (isMicroMessenger)
            {
                state = Guid.NewGuid().ToString();
                _properties.GetOrCreate(state, entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromSeconds(120));
                    return entry.Value = properties;
                });
            }

            parameters["state"] = state;

            return QueryHelpers.AddQueryString(authorizationEndpoint, parameters);
        }

        /// <summary>
        /// references to: 
        /// https://github.com/OrchardCMS/OrchardCore/blob/main/src/OrchardCore.Modules/OrchardCore.GitHub/Configuration/GithubHandler.cs#L23
        /// </summary>
        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            var parameters = new Dictionary<string, string>
            {
                { "access_token", tokens.AccessToken },
                { "openid", tokens.Response.RootElement.GetString("unionid") ?? tokens.Response.RootElement.GetString("openid") },
            };
            
            var request = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString(Options.UserInformationEndpoint, parameters));

            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"An error occurred when retrieving Wechat user information ({response.StatusCode}). Please check if the authentication information is correct in the corresponding Wechat Application.");
            }

            var payload = (await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync())).RootElement;

            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload);
            context.RunClaimActions();

            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        /// <summary>
        /// This code was copied from the aspnetcore repository . We should keep it in sync with it.
        /// https://github.com/dotnet/aspnetcore/blob/fcd4ed7c46083cc408417763867637f232928f9b/src/Security/Authentication/OAuth/src/OAuthHandler.cs#L193
        /// This can be removed or modified when the https://github.com/dotnet/aspnetcore/issues/33351 is resolved
        /// </summary>
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            var isMicroMessenger = IsMicroMessenger(Request);
            var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "appid", isMicroMessenger ? Options.AppId : Options.ClientId },
                { "redirect_uri", context.RedirectUri },
                { "secret", isMicroMessenger ? Options.AppSecret : Options.ClientSecret },
                { "code", context.Code },
                { "grant_type", "authorization_code" },
            };
            
            // PKCE https://tools.ietf.org/html/rfc7636#section-4.5, see BuildChallengeUrl
            if (context.Properties.Items.TryGetValue(OAuthConstants.CodeVerifierKey, out var codeVerifier))
            {
                tokenRequestParameters.Add(OAuthConstants.CodeVerifierKey, codeVerifier!);
                context.Properties.Items.Remove(OAuthConstants.CodeVerifierKey);
            }

            var requestContent = new FormUrlEncodedContent(tokenRequestParameters!);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = requestContent;
            requestMessage.Version = Backchannel.DefaultRequestVersion;
            var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                // This was added to support better error messages from the Wechat OAuth provider
                if (payload.RootElement.TryGetProperty("error", out var error))
                {
                    var output = new StringBuilder();
                    output.Append(error);

                    if (payload.RootElement.TryGetProperty("error_description", out var description))
                    {
                        output.Append(' ');
                        output.Append(description);
                    }
                    return OAuthTokenResponse.Failed(new Exception(output.ToString()));
                }

                return OAuthTokenResponse.Success(payload);
            }
            else
            {
                var error = "OAuth token endpoint failure: " + await Display(response);
                return OAuthTokenResponse.Failed(new Exception(error));
            }
        }

        private static async Task<string> Display(HttpResponseMessage response)
        {
            var output = new StringBuilder();
            output.Append("Status: " + response.StatusCode + ";");
            output.Append("Headers: " + response.Headers.ToString() + ";");
            output.Append("Body: " + await response.Content.ReadAsStringAsync() + ";");
            return output.ToString();
        }

        private bool IsMicroMessenger(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            return request.Headers["User-Agent"].ToString().ToLower().Contains("micromessenger") && Options.EnableOfficialAccount;
        }
    }
}
