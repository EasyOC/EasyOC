using AutoMapper;
using OrchardCore.Security;

namespace EasyOC.OrchardCore.OpenApi.Dto
{
    [AutoMap(typeof(RoleClaim), ReverseMap = true)]
    public class RoleClaimDto
    { 
        public string ClaimType { get; set; } 
        public string ClaimValue { get; set; }
    }
}
