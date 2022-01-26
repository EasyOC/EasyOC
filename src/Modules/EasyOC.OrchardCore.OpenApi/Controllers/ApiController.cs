using EasyOC.OrchardCore.OpenApi.Services;
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
            ///TODO: 执行导出逻辑
            ///TODO: 1. 获取类型名称，可以修改方法签名，但参数超过3个 需要封装成对象
            ///TODO: 2. 根据指定类型生成Excel Mapping结构
            ///TODO: 3. 从类型定义遍历属性取值，也就是把多维结构转换为平面结构 参考 TS 代码：ContentHelper.getAllFields, 可以把类似的类型解析逻辑迁移到  本项目的 Service 文件夹下 
            ///         也可参考： 但此处是处理用户扩展信息，内容类型比用户信息少一层 <seealso cref="UsersAppService.FillAdditionalData"/>
            ///TODO: 4  先构建处平面定义，包含一系列属性路径，属性名称 等，需要考虑不同Part下存在相同显示名称的字段
            ///TODO: 5  将内容写入到Excel    <seealso cref="UsersAppService.FillAdditionalData"/> 的 241行  有直接访问 JsonToken 的方法，支持Linq

            return null;
        }


    }
}



