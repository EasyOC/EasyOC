using AutoMapper;
using OrchardCore.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.WebApi.Dto
{
    [AutoMap(typeof(Permission))]
    public class PermissionDto
    {
        public const string ClaimType = "Permission";

        public string Name
        {
            get;
        }

        public string Description
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }

        public IEnumerable<PermissionDto> ImpliedBy
        {
            get;
        }
    }
}
