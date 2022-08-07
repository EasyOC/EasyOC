using AutoMapper;
using OrchardCore.Security;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.OpenApi.Dto
{
    [AutoMap(typeof(Role), ReverseMap = true)]
    public class UpdateRoleInput
    {
        public string RoleName
        {
            get;
            set;
        }

        public string RoleDescription
        {
            get;
            set;
        }

        public string NormalizedRoleName
        {
            get;
            set;
        }

        public List<RoleClaimDto> VbenMenuIds
        {
            get;
            set;
        } = new List<RoleClaimDto>();

        public List<RoleClaimDto> RoleClaims
        {
            get;
            set;
        } = new List<RoleClaimDto>();
    }
}
