using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;
using Flurl;
using Transformalize.Configuration;

namespace TransformalizeModule.Services.Transforms {
   public class RemoveQueryParameterTransform : StringTransform {

      private readonly Field _input;

      public RemoveQueryParameterTransform(
         IContext context = null
      ) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("string")) {
            return;
         }

         if (IsMissing(Context.Operation.Name)) {
            return;
         }

         _input = SingleInput();

      }

      public override IRow Operate(IRow row) {
         var value = GetString(row, _input);
         if (Url.IsValid(value)) {
            var url = new Url(value);
            url = url.RemoveQueryParam(Context.Operation.Name);
            row[Context.Field] = url.ToString();
         } else {
            row[Context.Field] = value;
         }
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("removequeryparam") {
            Parameters = new List<OperationParameter> { new OperationParameter("name") }
         };
         yield return new OperationSignature("removequeryparameter") {
            Parameters = new List<OperationParameter> { new OperationParameter("name") }
         };
      }
   }
}
