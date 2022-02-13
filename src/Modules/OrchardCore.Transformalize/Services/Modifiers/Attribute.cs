using Cfg.Net.Contracts;

namespace TransformalizeModule.Services.Modifiers {

   public class Attribute : IAttribute {
      public Attribute(string name, string value) {
         Name = name;
         Value = value;
      }

      public string Name { get; set; }
      public object Value { get; set; }
   }

}