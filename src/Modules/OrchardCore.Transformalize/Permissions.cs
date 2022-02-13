using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransformalizeModule {

   public class Permissions : IPermissionProvider {

      public static readonly Permission ManageTransformalizeSettings = new Permission(
          nameof(ManageTransformalizeSettings),
          "Manage Transformalize Settings"
      );

      public static readonly Permission AllowApi = new Permission(
         nameof(AllowApi),
         "Allow API"
      );

      public Task<IEnumerable<Permission>> GetPermissionsAsync() =>
         Task.FromResult(new[] {
            ManageTransformalizeSettings,
            AllowApi
         }
         .AsEnumerable()
      );

      public IEnumerable<PermissionStereotype> GetDefaultStereotypes() =>
          new[] {
               new PermissionStereotype {
                  Name = "Administrator",
                  Permissions = new[] { ManageTransformalizeSettings }
               }
          };
      }
}