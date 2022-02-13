using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace TransformalizeModule.Services.Transforms {
   public class GetDisplayUrlTransform : BaseTransform {

      private readonly string _url;

      public GetDisplayUrlTransform(
         IHttpContextAccessor httpContext = null,
         IContext context = null
      ) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         if (httpContext == null) {
            Run = false;
            Context.Error($"{nameof(GetDisplayUrlTransform)} requires an instance of IHttpContextAccessor");
         } else {
            _url = httpContext.HttpContext.Request.GetDisplayUrl();
         }

      }
      public override IRow Operate(IRow row) {
         row[Context.Field] = _url;
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("getdisplayurl");
      }
   }
}
