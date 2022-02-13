using NodaTime;
using System;
using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace TransformalizeModule.Services.Transforms {

   /// <summary>
   /// do not use yet.  fromTimeZone and toTimeZone are still validating against windows time zones
   /// </summary>
   public class OrchardTimeZoneTransform : BaseTransform {

      private readonly Transformalize.Configuration.Field _input;
      private readonly Func<DateTime, DateTime> _transform;

      public OrchardTimeZoneTransform(
         IContext context = null
      ) : base(context, "datetime") {

         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("date")) {
            return;
         }

         _input = SingleInput();

         if (IsMissing(Context.Operation.FromTimeZone)) {
            return;
         }

         if (IsMissing(Context.Operation.ToTimeZone)) {
            return;
         }

         var fromTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(Context.Operation.FromTimeZone);

         if (fromTimeZone == null) {
            Run = false;
            Context.Error($"The from time zone id {Context.Operation.FromTimeZone} is invalid. See IANA time zones.");
         }

         var toTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(Context.Operation.ToTimeZone);

         if (toTimeZone == null) {
            Run = false;
            Context.Error($"The to time zone id {Context.Operation.ToTimeZone} is invalid. See IANA time zones.");
         }

         _transform = (dt) => {
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            return ConvertDateTimeToDifferentTimeZone(dt, fromTimeZone, toTimeZone);
         };

      }

      public override IRow Operate(IRow row) {
         var date = (DateTime)row[_input];
         row[Context.Field] = _transform(date);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[] { new OperationSignature("timezone") {
               Parameters = new List<OperationParameter>() {
                  new OperationParameter("from-time-zone"),
                  new OperationParameter("to-time-zone")
               }
            }
         };
      }

      // https://stackoverflow.com/questions/39208477/is-this-the-proper-way-to-convert-between-time-zones-in-nodatime
      public static DateTime ConvertDateTimeToDifferentTimeZone(DateTime dt,DateTimeZone fromZone, DateTimeZone toZone) {
         var from = LocalDateTime.FromDateTime(dt);
         var fromZoned = from.InZoneLeniently(fromZone);
         var toZoned = fromZoned.WithZone(toZone);
         var to = toZoned.LocalDateTime;
         return to.ToDateTimeUnspecified();
      }

   }
}
