using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace TransformalizeModule.Services.Transforms {
   public class ToLocalTimeTransform : BaseTransform {

      private readonly IClock _clock;
      private readonly Transformalize.Configuration.Field _input;
      private readonly Func<DateTime, DateTime> _transform;

      public ToLocalTimeTransform(
         IContext context = null,
         IClock clock = null,
         ILocalClock localClock = null
      ) : base(context, "datetime") {

         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("date")) {
            return;
         }

         _input = SingleInput();

         _clock = clock;

         var localTimeZone = localClock.GetLocalTimeZoneAsync().Result;

         _transform = (dt) => {
            if(dt.Kind != DateTimeKind.Utc) {
               dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }
            return _clock.ConvertToTimeZone(dt, localTimeZone).DateTime;
         };

      }

      public override IRow Operate(IRow row) {
         var date = (DateTime)row[_input];
         row[Context.Field] = _transform(date);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[] { new OperationSignature("tolocaltime") };
      }
   }
}
