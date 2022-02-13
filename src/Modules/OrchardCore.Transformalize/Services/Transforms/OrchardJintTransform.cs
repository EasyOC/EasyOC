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
using Cfg.Net.Contracts;
using Esprima;
using Jint;
using Microsoft.Extensions.Caching.Memory;
using OrchardCore.Environment.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;
using Transformalize.Transforms.Jint;
using TransformalizeModule.Models;

namespace TransformalizeModule.Services.Transforms {

   public class OrchardJintTransform : BaseTransform {

      private readonly Engine _jint = new Engine();
      private readonly ParameterMatcher _parameterMatcher = new ParameterMatcher();
      private readonly Dictionary<int, string> _errors = new Dictionary<int, string>();
      private readonly IMemoryCache _memoryCache;
      private readonly ISignal _signal;
      private readonly IReader _reader;

      public bool TryFirst { get; set; } = true;

      public OrchardJintTransform(IContext context = null, IReader reader = null, IMemoryCache memoryCache = null, ISignal signal = null) : base(context, null) {

         if (IsMissingContext()) {
            return;
         }

         Returns = Context.Field.Type;

         if (IsMissing(Context.Operation.Script)) {
            return;
         }

         _memoryCache = memoryCache;
         _signal = signal;
         _reader = reader;

      }

      public override IRow Operate(IRow row) {
         throw new NotImplementedException();
      }

      /// <summary>
      /// implementing operate on rows (instead of row) to allow loading of external (file based) scripts to load first
      /// </summary>
      /// <param name="rows"></param>
      /// <returns></returns>
      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {

         if (!Run)
            yield break;

         var key = string.Join(':', Context.Process.Id, Context.Entity.Alias, Context.Field.Alias, Context.Operation.Method, Context.Operation.Index);

         if (!_memoryCache.TryGetValue(key, out CachedJintTransform transform)) {

            transform = new CachedJintTransform();
            var scriptBuilder = new StringBuilder();
            var scriptReader = new ScriptReader(Context, _reader);

            // to support shorthand script (e.g. t="js(scriptName)")
            if (Context.Operation.Scripts.Count == 0) {
               var script = Context.Process.Scripts.FirstOrDefault(s => s.Name == Context.Operation.Script);
               if (script != null) {
                  Context.Operation.Script = scriptReader.Read(script);
               }
            }

            var tester = new ScriptTester(Context);

            if (tester.Passes(Context.Operation.Script)) {
               // automatic parameter binding
               if (!Context.Operation.Parameters.Any()) {
                  var parameters = _parameterMatcher.Match(Context.Operation.Script, Context.GetAllEntityFields());
                  foreach (var parameter in parameters) {
                     Context.Operation.Parameters.Add(new Parameter { Field = parameter, Entity = Context.Entity.Alias });
                  }
               }
            } else {
               Run = false;
            }

            // for js, always add the input parameter
            transform.Input = MultipleInput().Union(new[] { Context.Field }).Distinct().ToArray();

            if (Context.Process.Scripts.Any(s => s.Global && (s.Language == "js" || s.Language == Constants.DefaultSetting && s.File != null && s.File.EndsWith(".js", StringComparison.OrdinalIgnoreCase)))) {
               // load any global scripts
               foreach (var sc in Context.Process.Scripts.Where(s => s.Global && (s.Language == "js" || s.Language == Constants.DefaultSetting && s.File != null && s.File.EndsWith(".js", StringComparison.OrdinalIgnoreCase)))) {
                  var content = scriptReader.Read(Context.Process.Scripts.First(s => s.Name == sc.Name));
                  if (tester.Passes(content)) {
                     scriptBuilder.AppendLine(content);
                  } else {
                     Run = false;
                  }
               }
            }

            // load any specified scripts
            if (Context.Operation.Scripts.Any()) {
               foreach (var sc in Context.Operation.Scripts) {
                  var content = scriptReader.Read(Context.Process.Scripts.First(s => s.Name == sc.Name));
                  if (tester.Passes(content)) {
                     scriptBuilder.AppendLine(content);
                  } else {
                     Run = false;
                  }
               }
            }

            if(scriptBuilder.Length > 0) {
               scriptBuilder.AppendLine(Context.Operation.Script);
            } else {
               scriptBuilder.Append(Context.Operation.Script);
            }

            try {
               transform.Script = new JavaScriptParser(scriptBuilder.ToString(), new ParserOptions() { Tolerant = true }).ParseScript();
            } catch (ParserException ex) {
               Context.Error(ex.Message);
               Utility.CodeToError(Context, scriptBuilder.ToString());
               Run = false;
            }

            // any changes to content item will invalidate cache
            _memoryCache.Set(key, transform, _signal.GetToken(Common.GetCacheKey(Context.Process.Id)));

         }

         if (!Run)
            yield break;

         foreach (var row in rows) {
            foreach (var field in transform.Input) {
               _jint.SetValue(field.Alias, row[field]);
            }
            if (TryFirst) {
               try {
                  TryFirst = false;
                  var obj = _jint.Evaluate(transform.Script).ToObject();
                  var value = obj == null ? null : Context.Field.Convert(obj);
                  if (value == null && !_errors.ContainsKey(0)) {
                     Context.Error($"Jint transform in {Context.Field.Alias} returns null!");
                     _errors[0] = $"Jint transform in {Context.Field.Alias} returns null!";
                  } else {
                     row[Context.Field] = value;
                  }
               } catch (Jint.Runtime.JavaScriptException jse) {
                  if (!_errors.ContainsKey(jse.LineNumber)) {
                     Context.Error("Script: " + Context.Operation.Script.Replace("{", "{{").Replace("}", "}}"));
                     Context.Error(jse, "Error Message: " + jse.Message);
                     Context.Error("Variables:");
                     foreach (var field in transform.Input) {
                        Context.Error($"{field.Alias}:{row[field]}");
                     }
                     _errors[jse.LineNumber] = jse.Message;
                  }
               }
            } else {
               row[Context.Field] = Context.Field.Convert(_jint.Evaluate(transform.Script).ToObject());
            }

            yield return row;
         }
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("jint") {
            Parameters = new List<OperationParameter> { new OperationParameter("script") }
         };
         yield return new OperationSignature("js") {
            Parameters = new List<OperationParameter> { new OperationParameter("script") }
         };
         yield return new OperationSignature("javascript") {
            Parameters = new List<OperationParameter> { new OperationParameter("script") }
         };
      }

   }
}
