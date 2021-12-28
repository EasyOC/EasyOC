using AutoMapper;
using OrchardCore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.WebApi.Dto
{
    [AutoMap(typeof(RoleClaim), ReverseMap = true)]
    public class RoleClaimDto
    {
        //
        // 摘要:
        //     Gets or sets the claim type for this claim.
        public string ClaimType
        {
            get;
            set;
        }

        //
        // 摘要:
        //     Gets or sets the claim value for this claim.
        public string ClaimValue
        {
            get;
            set;
        }
    }
}
