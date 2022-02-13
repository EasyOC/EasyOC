namespace TransformalizeModule.Services {
   public class AdoFormCommands {

      public AdoFormCommands() {
         Create = string.Empty;
         Insert = string.Empty;
         Update = string.Empty;
         Select = string.Empty;
      }

      public string Create { get; set; }
      public string Insert { get; set; }
      public string Update { get; set; }
      public string Select { get; set; }

   }
}
