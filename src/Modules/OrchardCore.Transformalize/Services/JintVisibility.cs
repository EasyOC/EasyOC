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
using Esprima;
using Esprima.Ast;
using Jint;
using Jint.Runtime;
using Transformalize.Impl;

namespace TransformalizeModule.Services {

   public class JintVisibility {

      private readonly Engine _jint = new Engine();

      public JvResult Visible(JvRequest request) {

         Script script;
         var result = new JvResult();

         if(string.IsNullOrEmpty(request.Script)) {
            result.Visible = true;
            return result;
         };

         try {
            script = new JavaScriptParser(request.Script, new ParserOptions() { Tolerant = true }).ParseScript();
         } catch (ParserException ex) {
            result.ParserException = ex;
            result.Message = $"{ex.Message} at column {ex.Column}.";
            result.Faulted = true;
            return result;
         }

         foreach (var field in request.Row) {
            _jint.SetValue(field.Key, field.Value);
         }

         try {
            var cv = _jint.Evaluate(script);
            if (cv.IsBoolean()) {
               result.Visible = (bool)cv.ToObject();
               return result;
            } else {
               result.Faulted = true;
               result.Message = "The JavaScript did not return a bool (a true or false).";
               return result;
            }
         } catch (JavaScriptException jse) {
            result.Faulted = true;
            result.Message = $"{jse.Message} at column {jse.Column}.";
            result.JavaScriptException = jse;
         }

         return result;
      }

   }

   public class JvRequest {
      public CfgRow Row { get; set; }
      public string Script { get; set; }

      public JvRequest(CfgRow row, string script) {
         Row = row;
         Script = script ?? string.Empty;
      }
   }

   public class JvResult {

      public bool Visible { get; set; }
      public ParserException ParserException { get; set; }
      public JavaScriptException JavaScriptException { get; set; }

      public string Message { get; set; }

      public bool Faulted { get; set; }

      public JvResult() {
         Message = string.Empty;
         Visible = true;
         Faulted = false;
      }

      public override string ToString() {
         return Visible.ToString().ToLower();
      }
   }
}
