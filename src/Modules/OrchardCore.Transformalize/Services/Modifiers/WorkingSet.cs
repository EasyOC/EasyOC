using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;

namespace TransformalizeModule.Services.Modifiers {
   /// <summary>
   /// Gets/Creates Keys and Collection for a given node, collection name, and key (attribute) name
   /// </summary>
   public class WorkingSet {
      public HashSet<string> Keys { get; set; }
      public INode Collection { get; set; }

      public WorkingSet(INode container, string collectionName, string keyName) {
         Keys = new HashSet<string>();
         Collection =  container.SubNodes.FirstOrDefault(n => n.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase));

         if(Collection == null) {
            var c = new Node(collectionName);
            container.SubNodes.Add(c);
            Collection = c;
         }

         foreach (var node in Collection.SubNodes) {
            if (node.TryAttribute(keyName, out var field)) {
               Keys.Add(field.Value.ToString());
            }
         }
      }
   }
}