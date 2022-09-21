using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.RDBMS
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageRelationalDbTypes = new Permission(nameof(ManageRelationalDbTypes), "Manage Relational Database Types");
        public static readonly Permission SyncAllRelationalDbData = new Permission(nameof(SyncAllRelationalDbData), "Synchronization all relational Database");
        public static readonly Permission ManageScriptQueries = new Permission("ManageScriptQueries", "Manage Script Queries");


        public async Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            var list = new List<Permission> { ManageRelationalDbTypes, SyncAllRelationalDbData ,ManageScriptQueries};
            return await Task.FromResult(list);
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { ManageRelationalDbTypes, SyncAllRelationalDbData,ManageScriptQueries }
                }
            };
        }

    }
}



