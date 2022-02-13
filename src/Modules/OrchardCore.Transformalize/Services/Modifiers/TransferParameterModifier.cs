using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;

namespace TransformalizeModule.Services.Modifiers {

   /// <inheritdoc />
   public class TransferParameterModifier : ICustomizer {

      private const string ParametersElementName = "parameters";
      private const string ParameterNameAttribute = "name";
      private const string ParameterValueAttribute = "value";
      private const string ParameterInputAttribute = "input";

      /// <inheritdoc />
      /// <summary>
      /// This gets called for each node, after the parameters have been merged
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="node"></param>
      /// <param name="parameters"></param>
      /// <param name="logger"></param>
      public void Customize(string parent, INode node, IDictionary<string, string> parameters, ILogger logger) {
         return;
      }

      /// <summary>
      /// This gets called first to transfer the external parameters into the internal parameters
      /// </summary>
      /// <param name="root"></param>
      /// <param name="parameters"></param>
      /// <param name="logger"></param>
      public void Customize(INode root, IDictionary<string, string> parameters, ILogger logger) {

         var rootParameters = root.SubNodes.FirstOrDefault(n => n.Name.Equals(ParametersElementName, StringComparison.OrdinalIgnoreCase));
         if (rootParameters == null)
            return;
         if (rootParameters.SubNodes.Count == 0)
            return;

         TransferParameters(rootParameters.SubNodes, parameters);
      }

      private void TransferParameters(IEnumerable<INode> nodes, IDictionary<string, string> parameters) {

         foreach (var parameterNode in nodes) {

            if (parameterNode.TryAttribute(ParameterNameAttribute, out var nameAttribute)) {

               if (nameAttribute.Value != null) {

                  var name = nameAttribute.Value.ToString();
                  if (parameters.ContainsKey(name)) {

                     // respect the input attribute, if input is false, don't let it come in
                     if (parameterNode.TryAttribute(ParameterInputAttribute, out var inputAttr) && inputAttr.Value.Equals("false")) {
                        parameters.Remove(name);
                        continue;
                     }

                     if (parameterNode.TryAttribute(ParameterValueAttribute, out var valueAttr)) {
                        valueAttr.Value = parameters[name];
                     } else {
                        parameterNode.Attributes.Add(new Attribute("value", parameters[name]));
                     }

                     parameters.Remove(name);  // "consumes" parameter because it's going to get transformed or validated

                  }

               }

            }

         }
      
      }

   }
}