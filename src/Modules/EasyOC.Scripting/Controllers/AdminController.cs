using EasyOC.ContentExtensions.AppServices;
using EasyOC.RDBMS;
using EasyOC.RDBMS.Models;
using EasyOC.RDBMS.Services;
using EasyOC.Scripting.Queries.ScriptQuery;
using EasyOC.Scripting.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ContentPermissions=OrchardCore.ContentTypes.Permissions;

namespace EasyOC.Scripting.Controllers
{
    public class AdminController : Controller
    {
        private readonly IContentTypeManagementAppService _contentManagementAppService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IRDBMSAppService _rDbmsAppService;

        private readonly IScriptQueryService _scriptQueryService;
        public AdminController(
            IAuthorizationService authorizationService,
            IContentTypeManagementAppService contentManagementAppService,
         IRDBMSAppService rDbmsAppService, IScriptQueryService scriptQueryService)
        {
            _authorizationService = authorizationService;
            _contentManagementAppService = contentManagementAppService;
            _rDbmsAppService = rDbmsAppService;
            _scriptQueryService = scriptQueryService;
        }
        [HttpGet]
        public async Task<IActionResult> Query(string query)
        {
            query = String.IsNullOrWhiteSpace(query) ? "" : System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(query));
            return View(new AdminQueryViewModel
            {
                DecodedQuery = query
            });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Query(AdminQueryViewModel model)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageScriptQueries))
            {
                return Forbid();
            }

            if (String.IsNullOrWhiteSpace(model.DecodedQuery))
            {
                return Json(model);
            }

            if (string.IsNullOrEmpty(model.Parameters))
            {
                model.Parameters = "{ }";
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.Parameters);
            var result = await _scriptQueryService.ExcuteScriptQuery(new ScriptQueryTesting()
            {
                Name = "TempQueryOnRunPage",
                Scripts = model.DecodedQuery,
                ReturnDocuments = model.ReturnDocuments
            }, parameters);
            model.Result = result;
            model.Elapsed = stopwatch.Elapsed.Milliseconds;

            return Json(model);
        }


        // [HttpGet]
        // public async Task<IActionResult> CreateQuery(string id)
        // {
        //     if (!await _authorizationService.AuthorizeAsync(User, QueriesPermissions.ManageQueries))
        //     {
        //         return Forbid();
        //     }
        //
        //     var query = _querySources.FirstOrDefault(x => x.Name == id)?.Create();
        //
        //     if (query == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var model = new QueriesCreateViewModel
        //     {
        //         Editor = await _displayManager.BuildEditorAsync(query, updater: _updateModelAccessor.ModelUpdater, isNew: true),
        //         SourceName = id
        //     };
        //
        //     return View(model);
        // }
        // [HttpGet]
        // public async Task<IActionResult> CreateQuery()
        // {
        //     
        // }

      
        [HttpGet]
        public async Task<IActionResult> GetAllTablesAsync(QueryTablesDto queryTablesDto)
        {
            if (!await _authorizationService.AuthorizeAsync(User, ContentPermissions.EditContentTypes))
            {
                return Forbid();
            }
            return Json(await _rDbmsAppService.GetAllTablesAsync(queryTablesDto));
        }

     
        [HttpGet]
        public async Task<IActionResult> GetAllDbConnecton()
        {
            if (!await _authorizationService.AuthorizeAsync(User, ContentPermissions.EditContentTypes))
            {
                return Forbid();
            }
            return Json(await _rDbmsAppService.GetAllDbConnection());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTypes()
        {
            if (!await _authorizationService.AuthorizeAsync(User, ContentPermissions.ViewContentTypes))
            {
                return Forbid();
            }
            return Json(await _rDbmsAppService.GetAllDbConnection());
        }
        [HttpGet]
        public async Task<IActionResult> GetTypeDefinitionAsync(string name, bool withSettings = false)
        {
            if (!await _authorizationService.AuthorizeAsync(User, ContentPermissions.EditContentTypes))
            {
                return Forbid();
            }
            return Json(_contentManagementAppService.GetTypeDefinition(name, withSettings));
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}