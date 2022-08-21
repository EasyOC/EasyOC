using AutoMapper;
using AutoMapper.Configuration.Annotations;
using EasyOC.OpenApi.Mappers;
using EasyOC.OpenApi.Model;
using OrchardCore.Security;

namespace EasyOC.OpenApi.Dto
{
    [AutoMap(typeof(RoleClaim), ReverseMap = true)]
    public class RoleClaimDto
    {
        /// <summary>
        /// 角色Claim类型，
        /// <see cref="RoleClaimMapping"/>
        /// </summary>
        public RoleClaimType ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
