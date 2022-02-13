using System.Collections.Generic;
using Cfg.Net.Contracts;
using Cfg.Net.Parsers;

namespace TransformalizeModule.Services.Modifiers {

   /// <summary>
   /// This creates parameters and filters for every field marked with parameter facet or search
   /// </summary>
   public class ReportParameterModifier : ICustomizer {

      private readonly HashSet<object> _triggers = new HashSet<object> { "facet", "facets", "search" };
      public ReportParameterModifier() {
      }

      public void Customize(INode root, IDictionary<string, string> parameters, ILogger logger) {

         var parameterSet = new WorkingSet(root, "parameters", "name");
         var entitySet = new WorkingSet(root, "entities", "name");

         foreach (var entity in entitySet.Collection.SubNodes) {

            var filterSet = new WorkingSet(entity, "filter", "field");
            var fieldSet = new WorkingSet(entity, "fields", "name");

            // cycle through fields looking for automatic parameters
            foreach (var field in fieldSet.Collection.SubNodes) {

               if (field.TryAttribute("parameter", out var parameter)) {

                  if (_triggers.Contains(parameter.Value) && field.TryAttribute("name", out var name)) {

                     var multiple = parameter.Value.Equals("facets");

                     if (!parameterSet.Keys.Contains(name.Value.ToString())) {
                       
                        var p = new Node("add");
                        p.Attributes.Add(new NodeAttribute("name", name.Value));
                        p.Attributes.Add(new NodeAttribute("value", "*"));
                        p.Attributes.Add(new NodeAttribute("prompt", "true"));
                        if (multiple) {
                           p.Attributes.Add(new NodeAttribute("multiple", "true"));
                        }
                        
                        if(field.TryAttribute("label", out var label)) {
                           p.Attributes.Add(new NodeAttribute("label", label.Value));
                        }

                        parameterSet.Collection.SubNodes.Add(p);
                     }

                     if (!filterSet.Keys.Contains(name.Value.ToString())) {

                        var f = new Node("add");
                        f.Attributes.Add(new NodeAttribute("field", name.Value));
                        f.Attributes.Add(new NodeAttribute("value", $"@[{name.Value}]"));
                        f.Attributes.Add(new NodeAttribute("type", multiple ? "facet" : parameter.Value));

                        filterSet.Collection.SubNodes.Add(f);
                     }
                  }
               }
            }
         }
      }

      public void Customize(string collection, INode node, IDictionary<string, string> parameters, ILogger logger) {
      }

   }
}