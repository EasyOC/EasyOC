using System.Collections.Generic;

namespace EasyOC.OrchardCore.OpenApi.Dto
{
    public class RoleDetailsDto
    {
        public string Name { get; set; }

        public string RoleDescription { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    
        public IEnumerable<string> VbenMenuIds { get; set; }
    }
}
