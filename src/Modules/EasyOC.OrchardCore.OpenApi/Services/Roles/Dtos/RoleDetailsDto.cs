using System.Collections.Generic;

namespace EasyOC.OrchardCore.OpenApi.Dto
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
        public IEnumerable<string> VbenMenuIds { get; set; }
    }
}
