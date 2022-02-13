using Fluid;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.Environment.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;
using TransformalizeModule.Models;

namespace TransformalizeModule.Services.Transforms {
   public class OrchardFluidTransform : BaseTransform {

      private readonly Func<string, object> _convert;
      private readonly IMemoryCache _memoryCache;
      private readonly ISignal _signal;
      private readonly FluidParser _parser;

      public OrchardFluidTransform(IContext context = null, IMemoryCache memoryCache = null, ISignal signal = null) : base(context, null) {
         if (IsMissingContext()) {
            return;
         }

         _memoryCache = memoryCache;
         _signal = signal;
         _parser = new FluidParser();

         Returns = Context.Field.Type;

         if (IsMissing(Context.Operation.Template)) {
            Run = false;
            return;
         }

         if (Returns == "string") {
            _convert = o => (o.Trim('\n', '\r'));
         } else {
            _convert = o => Context.Field.Convert(o.Trim(' ', '\n', '\r'));
         }
      }

      public override IRow Operate(IRow row) {
         throw new NotImplementedException("Not implemented here so it can wait for file based templates to load.");
      }

      /// <summary>
      /// implementing operate on rows (instead of row) to allow loading of external (file based) templates first
      /// </summary>
      /// <param name="rows"></param>
      /// <returns></returns>
      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {

         if (!Run)
            yield break;

         var key = string.Join(':', Context.Process.Id, Context.Entity.Alias, Context.Field.Alias, Context.Operation.Method, Context.Operation.Index);

         if (!_memoryCache.TryGetValue(key, out CachedFluidTransform transform)) {

            transform = new CachedFluidTransform();

            var fileBasedTemplate = Context.Process.Templates.FirstOrDefault(t => t.Name == Context.Operation.Template);

            if (fileBasedTemplate != null) {
               Context.Operation.Template = fileBasedTemplate.Content;
            }

            var input = MultipleInput();
            var matches = Context.Entity.GetFieldMatches(Context.Operation.Template);
            transform.Input = input.Union(matches).ToArray();

            if (_parser.TryParse(Context.Operation.Template, out transform.Template)) {

               // any changes to content item will invalidate cache
               _memoryCache.Set(key, transform, _signal.GetToken(Common.GetCacheKey(Context.Process.Id)));

            } else {
               Context.Error("Failed to parse fluid template.");
               Utility.CodeToError(Context, Context.Operation.Template);
               yield break;
            }
         }

         var context = new TemplateContext();
         foreach (var row in rows) {
            foreach (var field in transform.Input) {
               context.SetValue(field.Alias, row[field]);
            }
            row[Context.Field] = _convert(transform.Template.Render(context));
            yield return row;
         }

      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("fluid") {
            Parameters = new List<OperationParameter>(1) {
               new OperationParameter("template")
            }
         };
      }
   }
}
