using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace TransformalizeModule.Services.Transforms {
   public class UsernameTransform : BaseTransform {

      private readonly string _username = string.Empty;

      public UsernameTransform(IHttpContextAccessor httpContext = null, IContext context = null) : base(context, "string") {
         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("string")) {
            return;
         }
         
         if (httpContext == null) {
            Run = false;
            Context.Error($"{nameof(UsernameTransform)} requires an instance of IHttpContextAccessor");
         } else {
            _username = httpContext.HttpContext.User?.Identity?.Name ?? "Anonymous";
         }
      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = _username;
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("username");
      }
   }
}
