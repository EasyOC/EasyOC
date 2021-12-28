using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.WebApi.Dto
{
    public class RoleDetailsDto
    {
        public string Name
        {
            get;
            set;
        }

        public string RoleDescription
        {
            get;
            set;
        }

        public IDictionary<string, IEnumerable<PermissionDto>> RoleCategoryPermissions
        {
            get;
            set;
        }

        public IEnumerable<string> EffectivePermissions
        {
            get;
            set;
        }

      
        public RoleDto Role
        {
            get;
            set;
        }
    }
}
