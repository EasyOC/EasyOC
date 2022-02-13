using OrchardCore.ContentFields.Fields;
using System;
using System.Collections.Generic;

namespace TransformalizeModule.Ext {

   public static class TextFieldExtensions {

      public static bool Enabled(this TextField f) {
         return f.Text != "0";
      }

      public static bool OverrideDefaults(this TextField f) {
         return !string.IsNullOrEmpty(f.Text);
      }

      public static IEnumerable<int> SplitIntegers(this TextField f, char delimiter) {
         var items = new List<int>();
         if (f.Text == null)
            return items;
         foreach (var size in f.Text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)) {
            if (int.TryParse(size, out int result)) {
               items.Add(result);
            }
         }
         return items;
      }

   }
}
