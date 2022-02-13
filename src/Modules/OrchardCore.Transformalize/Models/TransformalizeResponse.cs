using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.Title.Models;
using Transformalize.Configuration;

namespace TransformalizeModule.Models {

   public class TransformalizeResponse<T> {
      private ContentItem contentItem;

      public ContentItem ContentItem {
         get => contentItem;
         set {
            contentItem = value;
            if(value != null) {
               Title = contentItem.As<TitlePart>()?.Title;
            }
         }
      }

      public T Part { get; set; }
      public string Title { get; set; }
      public Process Process { get; set; }
      public ActionResult ActionResult { get; set; }
      public bool Valid { get; set; }
      public bool Fails() => !Valid;

      public TransformalizeResponse(string format = null) {
         // this provides a non-null Process and determines the default serializer (xml or json)
         Process = format switch {
            "json" => new Process("{ \"name\":\"Process\" }"),
            "xml" => new Process("<cfg name=\"Process\" />"),
            _ => new Process(),
         };
      }
   }
}
