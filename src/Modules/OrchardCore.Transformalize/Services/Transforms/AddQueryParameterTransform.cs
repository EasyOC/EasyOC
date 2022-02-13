using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;
using Flurl;
using Transformalize.Configuration;
using System;

namespace TransformalizeModule.Services.Transforms {
   public class AddQueryParameterTransform : StringTransform {

      private readonly Field _input;
      private readonly Field _field;
      private readonly string _name;
      private readonly string _value;
      private readonly Func<Url, Field, IRow, string, string, string> _transform;

      public AddQueryParameterTransform(
         IContext context = null
      ) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }


         if (IsNotReceiving("string")) {
            return;
         }

         _input = SingleInput();

         if (Context.Entity.TryGetField(Context.Operation.Value, out var f1)) {
            // a single field argument, use field alias and value
            _field = f1;
            _transform = (url, field, row, name, value) => {
               url.QueryParams.Add(field.Alias, GetString(row, field), isEncoded: true);
               return url.ToString();
            };
         } else {
            if (Context.Operation.Value.Contains(",")) {
               var split = Context.Operation.Value.Split(',');
               _name = split[0];
               _value = split[1];
               if (Context.Entity.TryGetField(_value, out var f2)) {
                  // second parameter is a field, use name and field
                  _field = f2;
                  _transform = (url, field, row, name, value) => {
                     url.QueryParams.Add(name, GetString(row, field), isEncoded: true);
                     return url.ToString();
                  };
               } else {
                  // neither parameter is a field, use name and value
                  _transform = (url, field, row, name, value) => {
                     url.QueryParams.Add(name, value, isEncoded: true);
                     return url.ToString();
                  };
               }
            } else {
               Run = false;
               Context.Error($"The {nameof(AddQueryParameterTransform)} expects a field, or a comma delimited name and field or value. The argument {Context.Operation.Value} in the {Context.Field.Alias} field is not valid.");
            }
         }

      }
      public override IRow Operate(IRow row) {
         var value = GetString(row, _input);
         if (Url.IsValid(value)) {
            var url = new Url(value);
            row[Context.Field] = _transform(url, _field, row, _name, _value);
         } else {
            row[Context.Field] = value;
         }
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("addqueryparam") {
            Parameters = new List<OperationParameter> { new OperationParameter("value") }
         };
         yield return new OperationSignature("addqueryparameter") {
            Parameters = new List<OperationParameter> { new OperationParameter("value") }
         };
      }
   }
}
