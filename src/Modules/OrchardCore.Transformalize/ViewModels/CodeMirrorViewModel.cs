namespace TransformalizeModule.ViewModels {
   public class CodeMirrorViewModel {

      public CodeMirrorViewModel(string textAreaId, double portion) {
         TextAreaId = textAreaId;
         Portion = portion;
      }
      public string TextAreaId { get; set; }
      public double Portion { get; set; }
   }
}
