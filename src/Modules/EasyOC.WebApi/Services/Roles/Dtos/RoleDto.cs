using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrchardCore.Security;

namespace EasyOC.WebApi.Dto
{
    [AutoMap(typeof(IRole), ReverseMap = true)]
    public class RoleDto
    {
      public  string RoleName
        {
            get; set;
        }

        public string RoleDescription
        {
            get; set;
        }
    }
}
