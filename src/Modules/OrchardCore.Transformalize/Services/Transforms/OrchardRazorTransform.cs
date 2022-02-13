#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Microsoft.Extensions.Caching.Memory;
using OrchardCore.Environment.Cache;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;
using TransformalizeModule.Models;

namespace TransformalizeModule.Services.Transforms {

   public class OrchardRazorTransform : BaseTransform {

      private readonly Func<string, object> _convert;
      private readonly IMemoryCache _memoryCache;
      private readonly ISignal _signal;

      public OrchardRazorTransform(IContext context = null, IMemoryCache memoryCache = null, ISignal signal = null) : base(context, null) {

         if (IsMissingContext()) {
            return;
         }

         _memoryCache = memoryCache;
         _signal = signal;

         Returns = Context.Field.Type;

         IsMissing(Context.Operation.Template);

         if (Returns == "string") {
            _convert = o => (o.Trim('\n', '\r'));
         } else {
            _convert = o => Context.Field.Convert(o.Trim(' ', '\n', '\r'));
         }

      }

      public override IRow Operate(IRow row) {
         throw new NotImplementedException();
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

         if (!_memoryCache.TryGetValue(key, out CachedRazorTransform transform)) {

            transform = new CachedRazorTransform();

            var fileBasedTemplate = Context.Process.Templates.FirstOrDefault(t => t.Name == Context.Operation.Template);

            if (fileBasedTemplate != null) {
               Context.Operation.Template = fileBasedTemplate.Content;
            }

            var input = MultipleInput();
            var matches = Context.Entity.GetFieldMatches(Context.Operation.Template);
            transform.Input = input.Union(matches).ToArray();

            var engine = new RazorEngine();

            try {
               transform.Template = engine.Compile(Context.Operation.Template, builder => {
                  builder.AddUsing("System");
               });

               // any changes to content item will invalidate cache
               _memoryCache.Set(key, transform, _signal.GetToken(Common.GetCacheKey(Context.Process.Id)));

            } catch (RazorEngineCompilationException ex) {
               
               foreach (var error in ex.Errors) {
                  var line = error.Location.GetLineSpan();
                  Context.Error($"C# error on line {line.StartLinePosition.Line}, column {line.StartLinePosition.Character}.");
                  Context.Error(error.GetMessage());
               }
               Context.Error(ex.Message.Replace("{", "{{").Replace("}", "}}"));
               Utility.CodeToError(Context, Context.Operation.Template);
               yield break;
            }

         }

         foreach (var row in rows) {
            var output = transform.Template.Run(row.ToFriendlyExpandoObject(transform.Input));
            row[Context.Field] = _convert(output);
            yield return row;
         }

      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("razor") {
            Parameters = new List<OperationParameter> {
               new OperationParameter("template")
            }
         };
      }
   }
}