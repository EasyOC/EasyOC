using Dapper;
using EasyOC.OrchardCore.DynamicTypeIndex.Service;
using EasyOC.OrchardCore.FreeSql.Queries;
using Fluid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrchardCore.Liquid;
using OrchardCore.Modules;
using OrchardCore.Queries.Sql;
using EasyOC.OrchardCore.FreeSql.ViewModels;
using FreeSql.Aop;
using Natasha.CSharp;
using OrchardCore.ContentManagement.Records;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using YesSql;


namespace EasyOC.OrchardCore.FreeSql.Controllers
{
    [Feature("EasyOC.OrchardCore.FreeSql")]
    public class AdminController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IStore _store;
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly IStringLocalizer S;
        private readonly TemplateOptions _templateOptions;
        private readonly IDynamicIndexAppService _dynamicIndexAppService;
        private readonly IFreeSql freeSql;

        public AdminController(
            IAuthorizationService authorizationService,
            IStore store,
            ILiquidTemplateManager liquidTemplateManager,
            IStringLocalizer<AdminController> stringLocalizer,
            IOptions<TemplateOptions> templateOptions, IDynamicIndexAppService dynamicIndexAppService, IFreeSql freeSql)

        {
            _authorizationService = authorizationService;
            _store = store;
            _liquidTemplateManager = liquidTemplateManager;
            S = stringLocalizer;
            _dynamicIndexAppService = dynamicIndexAppService;
            this.freeSql = freeSql;
            _templateOptions = templateOptions.Value;
        }

        public Task<IActionResult> Query(string query)
        {
            query = String.IsNullOrWhiteSpace(query)
                ? ""
                : System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(query));
            return Query(new AdminQueryViewModel
            {
                DecodedQuery = query, FactoryName = _store.Configuration.ConnectionFactory.GetType().FullName
            });
        }

        [HttpPost]
        public async Task<IActionResult> Query(AdminQueryViewModel model)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageSqlQueries))
            {
                return Forbid();
            }

            if (String.IsNullOrWhiteSpace(model.DecodedQuery))
            {
                return View(model);
            }

            if (String.IsNullOrEmpty(model.Parameters))
            {
                model.Parameters = "{ }";
            }

            model.FactoryName = _store.Configuration.ConnectionFactory.GetType().FullName;

            var stopwatch = new Stopwatch();
            stopwatch.Start();


            var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.Parameters);

            var scripts = model.DecodedQuery;
            var builder = await _dynamicIndexAppService.GetIndexAssemblyBuilder(true);
            builder.Domain.UsingRecorder.Using(new[]
            {
                "OrchardCore.ContentManagement.Records", "FreeSql", "EasyOC.OrchardCore.FreeSql.Queries"
            });
            var curdBefore =
                new EventHandler<CurdBeforeEventArgs>((sender, e) =>
                {
                    model.RawSql = e.Sql;
                });
            try
            {
                //编译查询
                var funcDelegate = FastMethodOperator.UseCompiler(builder)
                    .Param<IFreeSql>(nameof(freeSql))
                    .Param<IDictionary<string, object>>(nameof(parameters))
                    .Return<FreeSqlQueryResults>()
                    .Body(scripts)
                    .Compile<Func<IFreeSql, IDictionary<string, object>, FreeSqlQueryResults>>();

                freeSql.Aop.CurdBefore += curdBefore;
                var sqlQueryResults = funcDelegate.Invoke(freeSql, parameters);
                //Sample Codes
                // var query = freeSql.Select<ContentItemIndex>();
                // model.RawSql = query.ToSql();

                // var result = new FreeSqlQueryResults { TotalCount = query.Count(), Items = query.ToList() };
                model.Count = (int)sqlQueryResults.TotalCount;
                model.Documents = sqlQueryResults.Items;
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", S["An error occurred while executing the SQL query: {0}", e.Message]);
            }
            finally
            {
                freeSql.Aop.CurdBefore -= curdBefore;
            }


            model.Elapsed = stopwatch.Elapsed;

            return View(model);
        }
    }
}
