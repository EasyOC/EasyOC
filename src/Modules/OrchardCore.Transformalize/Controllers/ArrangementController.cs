using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TransformalizeModule.Services;
using TransformalizeModule.Models;
using TransformalizeModule.Services.Contracts;
using Transformalize.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace TransformalizeModule.Controllers {

   [Authorize]
   public class ArrangementController : Controller {

      private readonly CombinedLogger<ArrangementController> _logger;
      private readonly ICommonService _commonService;
      private readonly ITransformalizeParametersModifier _modifier;
      private readonly IParameterService _parameterService;

      public ArrangementController(
         CombinedLogger<ArrangementController> logger,
         ICommonService common,
         ITransformalizeParametersModifier modifier,
         IParameterService parameterService
      ) {
         _logger = logger;
         _commonService = common;
         _modifier = modifier;
         _parameterService = parameterService;
      }

      [HttpGet]
      public async Task<ActionResult> TransformalizeParameters(string contentItemId) {

         var request = new TransformalizeRequest(contentItemId, HttpContext.User.Identity.Name) { Format = "xml" };

         var item = await _commonService.Validate(request);

         string arrangement;
         if (item.ContentItem.Has("TransformalizeTaskPart")) {
            arrangement = item.ContentItem.Content.TransformalizeTaskPart.Arrangement.Arrangement.Value;
         } else {
            arrangement = item.ContentItem.Content.TransformalizeReportPart.Arrangement.Arrangement.Value;
         }

         var process = new Process(_modifier.Modify(arrangement, item.ContentItem.Id, _parameterService.GetParameters()));
         process.Connections.Clear();

         if (item.Fails()) {
            return item.ActionResult;
         }

         return new ContentResult() { Content = process.Serialize(), ContentType = request.ContentType };
      }

   }
}
