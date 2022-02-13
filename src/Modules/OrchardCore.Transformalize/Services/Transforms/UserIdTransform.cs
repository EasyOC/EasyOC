using Microsoft.AspNetCore.Http;
using OrchardCore.Users.Models;
using OrchardCore.Users.Services;
using System;
using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace TransformalizeModule.Services.Transforms {
   public class UserIdTransform : BaseTransform {

      private readonly int _userId;

      public UserIdTransform(
         IHttpContextAccessor httpContext = null,
         IUserService userService = null,
         IContext context = null
      ) : base(context, "int") {

         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("int")) {
            return;
         }
         
         if (httpContext == null) {
            Run = false;
            Context.Error($"{nameof(UserIdTransform)} requires an instance of IHttpContextAccessor");
         } else {
            var username = httpContext.HttpContext.User?.Identity?.Name ?? "Anonymous";
            if(username != "Anonymous") {
               if (userService.GetUserAsync(username).Result is User user) {
                  _userId = user.Id;
               }
            }
         }

      }
      public override IRow Operate(IRow row) {
         row[Context.Field] = _userId;
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("userid");
      }
   }
}
