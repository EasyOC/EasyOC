using AutoMapper;
using OrchardCore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.WebApi.Dto
{
    [AutoMap(typeof(Role),ReverseMap = true)]
    public class RoleDetailsDto
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

        public List<RoleClaimDto> RoleClaims
        {
            get;
            set;
        } = new List<RoleClaimDto>();
    }
}
