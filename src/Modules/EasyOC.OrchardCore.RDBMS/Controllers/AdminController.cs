using EasyOC.OrchardCore.RDBMS.Models;
using EasyOC.OrchardCore.RDBMS.Services;
using EasyOC.OrchardCore.RDBMS.ViewModels;
using FreeSql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Records;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Settings;
using System.Collections.Generic;
using System.Linq;
using OrchardCore.ContentTypes;
using System.Threading.Tasks;
using YesSql;
using EasyOC.OrchardCore.ContentExtentions.AppServices;
using System;

namespace EasyOC.OrchardCore.RDBMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly IContentTypeManagementAppService _contentManagementAppService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly INotifier _notifier;
        private readonly IUpdateModelAccessor _updateModelAccessor;
        private readonly IStringLocalizer S;
        private readonly IHtmlLocalizer H;
        private readonly ISession _session;
        private readonly IContentFieldsValuePathProvider _contentFieldsValuePathProvider;
        private readonly IRDBMSAppService _rDBMSAppService;

        private readonly IServiceProvider _serviceProvider;

        public AdminController(
            IAuthorizationService authorizationService,
            ISiteService siteService,
            IStringLocalizer<AdminController> stringLocalizer,
            IHtmlLocalizer<AdminController> htmlLocalizer,
            INotifier notifier,
            IUpdateModelAccessor updateModelAccessor,
            IContentDefinitionManager contentDefinitionManager,
            IContentManager contentManager, ISession session, IContentFieldsValuePathProvider contentFieldsValuePathProvider, IRDBMSAppService rDBMSAppService, IContentTypeManagementAppService contentManagementAppService, IServiceProvider serviceProvider)
        {
            _authorizationService = authorizationService;
            _siteService = siteService;
            _updateModelAccessor = updateModelAccessor;
            _notifier = notifier;
            S = stringLocalizer;
            H = htmlLocalizer;
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _session = session;
            _contentFieldsValuePathProvider = contentFieldsValuePathProvider;
            _rDBMSAppService = rDBMSAppService;
            _contentManagementAppService = contentManagementAppService;
            _serviceProvider = serviceProvider;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrEditPost(RDBMSMappingConfigViewModel model)
        {
            ContentItem contentItem;
            if (string.IsNullOrEmpty(model.Id))
            {
                contentItem = await _contentManager.NewAsync("RDBMSMappingConfig");
            }
            else
            {
                contentItem = await _contentManager.GetAsync(model.Id);
            }
            var dbEntity = contentItem.As<RDBMSMappingConfig>();

            //     var contentItem = new ContentItem()
            //     {
            //         ContentType = "RDBMSMappingConfig",
            //         DisplayText=model.ConfigName, 
            // };
            return View(model);
        }
        [HttpGet]
        public IActionResult CreateOrEdit()
        {
            var model = new RDBMSMappingConfigViewModel();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTablesAsync(QueryTablesDto queryTablesDto)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditContentTypes))
            {
                return Forbid();
            }
            return Json(await _rDBMSAppService.GetAllTablesAsync(queryTablesDto));
        }

        [HttpGet]
        public async Task<RecipeModel> GenerateRecipeAsync(string tableName, string connectionConfigId)
        {

            var connectionObject = await _contentManager.GetAsync(connectionConfigId);

            IFreeSql freeSql = _serviceProvider.GetFreeSql((string)connectionObject.Content.DbConnectionConfig.ProviderName.Text.Value,
                (string)connectionObject.Content.DbConnectionConfig.ConnectionString.Text.Value);
            using (freeSql)
            {
                var recipe = new RecipeModel();

                var step = new Step();
                recipe.steps = new List<Step>() { step };
                step.name = "ContentDefinition";
                step.ContentTypes = new List<ContentType>();
                var contentType = new ContentType()
                {
                    Name = tableName,
                    DisplayName = tableName,
                    Settings = JObject.Parse(@"
                                            {'ContentTypeSettings': {
                                                'Creatable': true,
                                                  'Listable': true,
                                                  'Draftable': true,
                                                  'Versionable': true,
                                                  'Securable': true
                                            }}")


                };
                step.ContentTypes.Add(contentType);
                contentType.ContentTypePartDefinitionRecords = new ContentTypePartDefinitionRecord[]{ new ContentTypePartDefinitionRecord
                    {
                        Name = tableName,
                        PartName =tableName
                    }};

                var recrods = new List<ContentPartFieldDefinitionRecord>();
                try
                {

                    var tb = freeSql.Select<object>().AsTable((type, oldname) => tableName).First();
                    var Tbs = freeSql.DbFirst.GetTablesByDatabase();
                    foreach (var item in tb.GetType().GetProperties())
                    {
                        var recrod = new ContentPartFieldDefinitionRecord();
                        recrod.Name = item.Name;
                        recrod.Settings = JObject.FromObject(new
                        {
                            ContentPartFieldSettings = new { DisplayName = item.Name }
                        });
                        var targetFieldType = _contentFieldsValuePathProvider.GetField(item.PropertyType);
                        recrod.FieldName = targetFieldType.FieldName;
                        recrods.Add(recrod);
                    }

                    step.ContentParts.Add(new Contentpart { Name = tableName, ContentPartFieldDefinitionRecords = recrods.ToArray() });

                    return recipe;
                }
                catch (System.Exception)
                {
                    return null;
                }
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllDbConnecton()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditContentTypes))
            {
                return Forbid();
            }
            return Json(await _rDBMSAppService.GetAllDbConnecton());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTypes()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ViewContentTypes))
            {
                return Forbid();
            }
            return Json(await _rDBMSAppService.GetAllDbConnecton());
        }
        [HttpGet]
        public async Task<IActionResult> GetTypeDefinitionAsync(string name, bool withSettings = false)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditContentTypes))
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



