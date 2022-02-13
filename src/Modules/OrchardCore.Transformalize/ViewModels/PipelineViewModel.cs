using Microsoft.AspNetCore.Html;

namespace TransformalizeModule.ViewModels {
   public class PipelineViewModel {
      private string _label;
      private string _title;

      public PipelineViewModel(string mode) {
         Mode = mode;
      }
      public string Mode { get; set; }
      public bool Active { get; set; }
      public string Title { get { return _title == null ? Mode + " view" : _title; } set { _title = value; } }
      public string Label { get { return _label == null ? Mode.ToUpper() : _label; } set { _label = value; }}
      public HtmlString Link { get; set; }
      public string Glyphicon { get; set; }
   }
}
