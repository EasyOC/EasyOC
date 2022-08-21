using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace OrchardCore.Users.Services
{
    internal class UserTokenLifeTimeClaimsProvider : IUserClaimsProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserTokenLifeTimeClaimsProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task GenerateAsync(IUser user, ClaimsIdentity claims)
        {
            var lifeTime = TimeSpan.FromHours(10);
            var request = _httpContextAccessor.HttpContext.Request;
            if (request.Path.Value.ToLower().EndsWith("token") && request.Method.ToUpper() == "POST")
            {
                if (request.Form["rememberMe"] == "true")
                {
                    lifeTime = TimeSpan.FromDays(7);
                }
                claims.AddClaim(new Claim("oi_act_lft",
                   lifeTime.TotalSeconds.ToString(CultureInfo.InvariantCulture)));

            }
            return Task.FromResult(claims);
        }

    }

}
