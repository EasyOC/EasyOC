using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;
using Flurl;
using Transformalize.Configuration;

namespace TransformalizeModule.Services.Transforms {
   public class RemoveQueryParametersTransform : StringTransform {

      private readonly Field _input;

      public RemoveQueryParametersTransform(
         IContext context = null
      ) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("string")) {
            return;
         }

         _input = SingleInput();

      }

      public override IRow Operate(IRow row) {
         var value = GetString(row, _input);
         if (Url.IsValid(value)) {
            var url = new Url(value);
            url.QueryParams.Clear();
            row[Context.Field] = url.ToString();
         } else {
            row[Context.Field] = value;
         }
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("removequeryparams");
         yield return new OperationSignature("removequeryparameters");
      }
   }
}
