using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TransformalizeModule.Services.Contracts;
using TransformalizeModule.Services;
using TransformalizeModule.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using OrchardCore.DisplayManagement.Notify;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using Autofac;
using IContainer = TransformalizeModule.Services.Contracts.IContainer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace TransformalizeModule.Controllers {

   [Authorize]
   public class FormController : Controller {

      private readonly CombinedLogger<FormController> _logger;
      private readonly IFormService _formService;
      private readonly INotifier _notifier;
      private readonly IHtmlLocalizer<FormController> H;
      private readonly IHttpContextAccessor _httpContext;
      private readonly IContainer _container;

      public FormController(
         IFormService formService,
         INotifier notifier,
         IContainer container,
         IHttpContextAccessor httpContext,
         IHtmlLocalizer<FormController> htmlLocalizer,
         CombinedLogger<FormController> logger
      ) {
         _logger = logger;
         _formService = formService;
         _httpContext = httpContext;
         _container = container;
         _notifier = notifier;
         H = htmlLocalizer;
      }

      public async Task<ActionResult> Index(string contentItemId) {

         var form = await _formService.ValidateForm(
            new TransformalizeRequest(contentItemId, HttpContext.User.Identity.Name) {
               InternalParameters = Common.GetFileParameters(Request)
            }
         );

         if (form.Fails()) {
            return form.ActionResult;
         }

         if (Request.Method == "POST" && _httpContext.HttpContext.Request.HasFormContentType) {

            if (!form.Process.Parameters.All(p => p.Valid)) {
               _notifier.Error(H["The form did not pass validation.  Please correct it and re-submit."]);
               return View(form);
            }

            var scope = _container.CreateScope(form.Process, _logger, null);

            if (!scope.IsRegistered<AdoFormCommandWriter>()) {
               _logger.Error(() => "The form command writer isn't registered.");
               return View(form);
            }

            if (!form.Process.Parameters.Any(p => p.PrimaryKey)) {
               _notifier.Error(H["To save the form, you must specify one of the parameters as the primary key."]);
               return View(form);
            }

            var commands = scope.Resolve<AdoFormCommandWriter>().Write();
            var key = form.Process.Parameters.First(k => k.PrimaryKey);
            var insert = key.Value == Transformalize.Constants.TypeDefaults()[key.Type].ToString();

            // reset, modify for actual insert, and execute
            form.Process.Actions.Add(new Transformalize.Configuration.Action {
               After = true,
               Before = false,
               Type = "run",
               Connection = form.Process.Connections.First().Name,
               Command = insert ? commands.Insert : commands.Update,
               Key = Guid.NewGuid().ToString(),
               ErrorMode = "exception"
            });

            try {
               _formService.Run(form.Process);
               _notifier.Information(insert ? H["{0} inserted", form.Process.Name] : H["{0} updated", form.Process.Name]);
               if (Request.Form["ReturnUrl"] != StringValues.Empty) {
                  return Redirect(Request.Form["ReturnUrl"].ToString());
               }
            } catch (Exception ex) {
               if (ex.Message.Contains("duplicate")) {
                  _notifier.Error(H["The {0} save failed: {1}", form.Process.Name, "The database has rejected this update due to a unique constraint violation."]);
               } else {
                  _notifier.Error(H["The {0} save failed: {1}", form.Process.Name, ex.Message]);
               }
               _logger.Error(ex, () => ex.Message);
            }

         }

         return View(form);
      }

      public async Task<ActionResult> Form(string contentItemId) {

         var form = await _formService.ValidateForm(
            new TransformalizeRequest(contentItemId, HttpContext.User.Identity.Name) {
               InternalParameters = Common.GetFileParameters(Request)
            }
         );

         if (form.Fails()) {
            return form.ActionResult;
         }

         return View("Form", form.Process);
      }

      [HttpGet]
      public async Task<ActionResult> Run(string contentItemId, string format = "json") {

         var request = new TransformalizeRequest(contentItemId, HttpContext.User.Identity.Name) { 
            Format = format,
            InternalParameters = Common.GetFileParameters(Request)
         };
         var report = await _formService.ValidateForm(request);

         if (report.Fails()) {
            return report.ActionResult;
         }

         await _formService.RunAsync(report.Process);

         report.Process.Connections.Clear();

         return new ContentResult() { Content = report.Process.Serialize(), ContentType = request.ContentType };
      }

   }
}
