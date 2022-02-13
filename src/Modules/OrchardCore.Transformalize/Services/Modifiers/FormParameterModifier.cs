using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;
using Cfg.Net.Parsers;

namespace TransformalizeModule.Services.Modifiers {

   /// <summary>
   /// This creates parameters for every input field
   /// TODO: Use the WorkingSet(s) to simplify code
   /// </summary>
   public class FormParameterModifier : ICustomizer {

      public void Customize(string collection, INode node, IDictionary<string, string> parameters, ILogger logger) { }

      public void Customize(INode root, IDictionary<string, string> parameters, ILogger logger) {

         var parameterCollection = root.SubNodes.FirstOrDefault(n => n.Name.Equals("parameters", StringComparison.OrdinalIgnoreCase));

         if (parameterCollection != null) {

            var parameterNames = new HashSet<string>();

            foreach (var node in parameterCollection.SubNodes) {
               if (node.TryAttribute("name", out var name)) {
                  parameterNames.Add(name.Value.ToString());
               }
            }

            var entityCollection = root.SubNodes.FirstOrDefault(n => n.Name.Equals("entities", StringComparison.OrdinalIgnoreCase));
            if (entityCollection != null) {
               foreach (var entity in entityCollection.SubNodes) {
                  var fieldCollection = entity.SubNodes.FirstOrDefault(n => n.Name.Equals("fields", StringComparison.OrdinalIgnoreCase));
                  if (fieldCollection != null) {
                     foreach (var field in fieldCollection.SubNodes) {
                        if (field.TryAttribute("name", out var name)) {

                           var add = false;
                           if (field.TryAttribute("input", out var input)) {
                              if (input.Value.ToString().ToLower() == "true") {
                                 add = true;
                              }
                           } else {
                              add = true;
                           }

                           if (!parameterNames.Contains(name.Value) && add) {

                              var node = new Node("add");
                              node.Attributes.Add(new NodeAttribute("name", name.Value));
                              if (field.TryAttribute("default", out var def)) {
                                 NodeAttribute n = new NodeAttribute("value", null);
                                 if (def.Value.ToString() == Transformalize.Constants.DefaultSetting) {
                                    n.Value = string.Empty;
                                 } else {
                                    n.Value = def.Value;
                                 }
                                 node.Attributes.Add(n);
                              } else {
                                 node.Attributes.Add(new NodeAttribute("value", string.Empty));
                              }

                              node.Attributes.Add(new NodeAttribute("label", "n/a"));
                              node.Attributes.Add(new NodeAttribute("invalid-characters", string.Empty));

                              if (field.TryAttribute("type", out var type)) {
                                 if (type != null && type.Value != null && type.Value.ToString().ToLower().StartsWith("bool")) {
                                    node.Attributes.Add(new NodeAttribute("type", "bool"));
                                 }
                              }

                              parameterCollection.SubNodes.Add(node);

                           }
                        }


                     }
                  }
               }
            }
         }
      }
   }
}