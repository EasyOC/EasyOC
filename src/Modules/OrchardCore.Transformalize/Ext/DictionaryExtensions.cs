using System;
using System.Collections.Generic;

namespace TransformalizeModule.Ext {
   public static class DictionaryExtensions {
      public static int GetIntegerOrDefault(this IDictionary<string,string> dict, string name, Func<int> getDefault) {
         if (!dict.ContainsKey(name)) {
            return getDefault();
         }
         if (int.TryParse(dict[name], out int value)) {
            return value;
         } else {
            return getDefault();
         }
      }
   }
}
