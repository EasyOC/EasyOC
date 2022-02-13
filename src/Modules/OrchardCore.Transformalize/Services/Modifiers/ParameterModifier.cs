using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;
using Cfg.Net.Environment;

namespace TransformalizeModule.Services.Modifiers {

   /// <summary>
   /// This modifier is responsible for gathering the external and cfg.Parameters together into the parameters dictionary
   /// and replacing them throughout the configuration (before it's loaded)
   /// </summary>
   public class ParameterModifier : ICustomizer {

      private readonly IPlaceHolderReplacer _placeHolderReplacer;
      private const string ParametersElementName = "parameters";
      private const string ParameterNameAttribute = "name";
      private const string ParameterValueAttribute = "value";
      private const string ParameterInputAttribute = "input";

      public ParameterModifier(
          IPlaceHolderReplacer placeHolderReplacer
          ) {
         _placeHolderReplacer = placeHolderReplacer;
      }

      /// <inheritdoc />
      /// <summary>
      /// This runs the parameter place-holder replacer on each attribute in each node
      /// This happens after the parameters have been merged
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="node"></param>
      /// <param name="parameters"></param>
      /// <param name="logger"></param>
      public void Customize(string parent, INode node, IDictionary<string, string> parameters, ILogger logger) {
         if (parameters.Count == 0)
            return;

         foreach (var attribute in node.Attributes) {
            attribute.Value = _placeHolderReplacer.Replace(attribute.Value.ToString(), parameters, logger);
         }
      }

      /// <summary>
      /// This gets called first to merge the parameters that are passed in with the defaults
      /// They are "merged" into the parameters dictionary
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

         MergeParameters(rootParameters.SubNodes, parameters);
      }

      private void MergeParameters(IEnumerable<INode> nodes, IDictionary<string, string> parameters) {
         foreach (var parameter in nodes) {
            string name = null;
            object value = null;
            foreach (var attribute in parameter.Attributes) {
               if (attribute.Name == ParameterNameAttribute) {
                  name = attribute.Value.ToString();
               } else if (attribute.Name == ParameterValueAttribute) {
                  value = attribute.Value;
               }
            }
            if (name != null && value != null) {


               if (parameters.ContainsKey(name)) {  
                  // arrangement and external parameter match

                  // if the arrangment parameter says input is false, we remove it as it is not permitted
                  if (parameter.TryAttribute(ParameterInputAttribute, out var inputAttr) && inputAttr.Value.Equals("false")) {
                     parameters.Remove(name);
                     continue;
                  }

                  // the external parameter will set the arrangement parameter's value attribute
                  if (parameter.TryAttribute(ParameterValueAttribute, out var valueAttr)) {
                     valueAttr.Value = parameters[name];
                  } else {
                     parameter.Attributes.Add(new Attribute("value", parameters[name]));
                  }

               } else { // attribute value is going to set the parameter
                  parameters[name] = value.ToString();
               }
            }
         }
      }

   }
}