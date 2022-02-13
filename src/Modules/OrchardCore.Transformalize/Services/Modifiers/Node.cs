using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace TransformalizeModule.Services.Modifiers {
   public class Node : INode {

      private Dictionary<string, IAttribute> _attributes;

      public Node(string name) {
         Name = name;
         Attributes = new List<IAttribute>();
         SubNodes = new List<INode>();
      }

      public string Name { get; }
      public List<IAttribute> Attributes { get; }
      public List<INode> SubNodes { get; }

      public bool TryAttribute(string name, out IAttribute attr) {
         if (_attributes == null) {
            _attributes = new Dictionary<string, IAttribute>();
            for (var i = 0; i < Attributes.Count; i++) {
               _attributes[Attributes[i].Name] = Attributes[i];
            }
         }
         if (_attributes.ContainsKey(name)) {
            attr = _attributes[name];
            return true;
         }
         attr = null;
         return false;
      }
   }
}