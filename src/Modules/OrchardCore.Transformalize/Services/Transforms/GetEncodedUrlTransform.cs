using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace TransformalizeModule.Services.Transforms {
   public class GetEncodedUrlTransform : BaseTransform {

      private readonly string _url;

      public GetEncodedUrlTransform(
         IHttpContextAccessor httpContext = null,
         IContext context = null
      ) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         if (httpContext == null) {
            Run = false;
            Context.Error($"{nameof(GetEncodedUrlTransform)} requires an instance of IHttpContextAccessor");
         } else {
            _url = httpContext.HttpContext.Request.GetEncodedUrl();
         }

      }
      public override IRow Operate(IRow row) {
         row[Context.Field] = _url;
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("getencodedurl");
      }
   }
}
