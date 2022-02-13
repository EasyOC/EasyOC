using System.Collections.Generic;
using System.Linq;
using Parameter = Transformalize.Configuration.Parameter;

namespace TransformalizeModule.Ext {

   public static class ParameterExtensions {

      public static bool Readonly(this Parameter p) {
         return !p.Prompt || p.Visible == "false";
      }

      public static bool VisiblePrompt(this Parameter p) {
         return p.Prompt && p.Visible != "false";
      }

      public static string ToParsley(this Parameter f) {
         if (f.V == string.Empty)
            return string.Empty;

         var attributes = new Dictionary<string, string>();

         var expressions = new Cfg.Net.Shorthand.Expressions(f.V);
         foreach (var expression in expressions) {
            switch (expression.Method) {
               case "required":
                  attributes["data-parsley-required"] = "true";
                  switch (f.InputType) {
                     case "file":
                     case "scan":
                        attributes["data-parsley-required-message"] = "a " + f.InputType + " is required";
                        break;
                     default:
                        break;
                  }
                  break;
               case "length":
                  attributes["data-parsley-length"] = string.Format("[{0}, {1}]", expression.SingleParameter, expression.SingleParameter);
                  break;
               case "numeric":
                  attributes["data-parsley-type"] = "number";
                  break;
               case "matches":
                  attributes["data-parsley-pattern"] = expression.SingleParameter;
                  break;
               case "min":
                  attributes["data-parsley-min"] = expression.SingleParameter;
                  break;
               case "max":
                  attributes["data-parsley-max"] = expression.SingleParameter;
                  break;
               case "is":
                  switch (expression.SingleParameter) {
                     case "int":
                     case "int32":
                        attributes["data-parsley-type"] = "integer";
                        break;
                     case "date":
                     case "datetime":
                        attributes["data-parsley-date"] = "true";
                        break;
                  }
                  break;
               case "alphanum":
                  attributes["data-parsley-type"] = "alphanum";
                  break;
               case "digits":
                  attributes["data-parsley-type"] = "digits";
                  break;
               case "email":
                  attributes["data-parsley-type"] = "email";
                  break;
               case "url":
                  attributes["data-parsley-type"] = "url";
                  break;
            }
         }


         return string.Join(" ", attributes.Select(i => string.Format("{0}=\"{1}\"", i.Key, i.Value)));
      }

      public static bool UseTextArea(this Parameter parameter, out int length) {
         var useTextArea = parameter.Length == "max";
         length = 4000;
         if (!useTextArea) {
            if (int.TryParse(parameter.Length, out length)) {
               useTextArea = length >= 255;
            }
         }
         if (length == 0) {
            length = 4000;
         }
         return useTextArea;
      }
   }
}