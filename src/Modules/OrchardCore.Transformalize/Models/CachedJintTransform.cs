using Transformalize.Configuration;

namespace TransformalizeModule.Models {
   public class CachedJintTransform {
      public Esprima.Ast.Script Script { get; set; }
      public Field[] Input;
   }

}