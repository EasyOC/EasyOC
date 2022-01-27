using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Lucene;
using OrchardCore.Queries;
using System.Threading.Tasks;
using SqlQuery = OrchardCore.Queries.Sql.SqlQuery;

namespace EasyOC.OrchardCore.OpenApi.Controllers
{
    [Route("api/Excel")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
    public class ApiController : Controller
    {
        private readonly IQueryManager _queryManager;

        public ApiController(IQueryManager queryManager)
        {
            _queryManager = queryManager;
        }

        public async Task<IActionResult> ExportAsync(string queryName, object parameters)
        {

            var query = await _queryManager.GetQueryAsync(queryName);
            var returnDocuments = false;
            if (query is LuceneQuery luceneQuery)
            {
                returnDocuments = luceneQuery.ReturnContentItems;
            }
            if (query is SqlQuery sqlQuery)
            {
                returnDocuments = sqlQuery.ReturnDocuments;
            }
            //

            return null;
        }


    }
}



