using System.Threading.Tasks;
using OrchardCore.Alias.Models;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;

namespace TransformalizeModule.Handlers {
   public class TransformalizeHandler : ContentHandlerBase {

      public override Task GetContentItemAspectAsync(ContentItemAspectContext context) {

         // change the view links to goto custom controllers

         switch (context.ContentItem.ContentType) {
            case "TransformalizeReport":
               return context.ForAsync<ContentItemMetadata>(metadata => {
                  if (metadata.DisplayRouteValues != null) {
                     metadata.DisplayRouteValues.Remove("Area");
                     metadata.DisplayRouteValues.Add("Area", Common.ModuleName);
                     metadata.DisplayRouteValues.Remove("Controller");
                     metadata.DisplayRouteValues.Add("Controller", "Report");
                     metadata.DisplayRouteValues.Remove("Action");
                     metadata.DisplayRouteValues.Add("Action", "Index");
                     metadata.DisplayRouteValues.Remove("ContentItemId");
                     metadata.DisplayRouteValues.Add("ContentItemId", context.ContentItem.As<AliasPart>().Alias);
                  }
                  return Task.CompletedTask;
               });
            case "TransformalizeTask":
               return context.ForAsync<ContentItemMetadata>(metadata => {
                  if (metadata.DisplayRouteValues != null) {
                     metadata.DisplayRouteValues.Remove("Area");
                     metadata.DisplayRouteValues.Add("Area", Common.ModuleName);
                     metadata.DisplayRouteValues.Remove("Controller");
                     metadata.DisplayRouteValues.Add("Controller", "Task");
                     metadata.DisplayRouteValues.Remove("Action");
                     metadata.DisplayRouteValues.Add("Action", "Review");
                     metadata.DisplayRouteValues.Remove("ContentItemId");
                     metadata.DisplayRouteValues.Add("ContentItemId", context.ContentItem.As<AliasPart>().Alias);
                  }
                  return Task.CompletedTask;
               });
            case "TransformalizeForm":
               return context.ForAsync<ContentItemMetadata>(metadata => {
                  if (metadata.DisplayRouteValues != null) {
                     metadata.DisplayRouteValues.Remove("Area");
                     metadata.DisplayRouteValues.Add("Area", Common.ModuleName);
                     metadata.DisplayRouteValues.Remove("Controller");
                     metadata.DisplayRouteValues.Add("Controller", "Form");
                     metadata.DisplayRouteValues.Remove("Action");
                     metadata.DisplayRouteValues.Add("Action", "Index");
                     metadata.DisplayRouteValues.Remove("ContentItemId");
                     metadata.DisplayRouteValues.Add("ContentItemId", context.ContentItem.As<AliasPart>().Alias);
                  }
                  return Task.CompletedTask;
               });
            default:
               return Task.CompletedTask;
         }
      }
   }
}
