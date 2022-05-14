using EasyOC.OrchardCore.ContentExtentions.AppServices;
using EasyOC.OrchardCore.ContentExtentions.Models;
using EasyOC.OrchardCore.OpenApi.Indexs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Lucene;
using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using YesSql.Services;
using SqlQuery = OrchardCore.Queries.Sql.SqlQuery;

namespace EasyOC.OrchardCore.OpenApi.Controllers
{
    [Route("api/Excel")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
    public class ApiController : Controller
    {
        private readonly IQueryManager _queryManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentTypeManagementAppService _contentManagementAppService;
        private readonly ISession _session;
        public ApiController(IQueryManager queryManager, IContentDefinitionManager contentDefinitionManager, ISession session, IContentTypeManagementAppService contentManagementAppService)
        {
            _queryManager = queryManager;
            _contentDefinitionManager = contentDefinitionManager;
            _session = session;
            _contentManagementAppService = contentManagementAppService;
        }
        [HttpGet]
        public async Task<IActionResult> ExportAsync(string queryName, IDictionary<string, object> parameters, string typeName = default)
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

            var contentTypeName = typeName;
            if (!contentTypeName.IsNullOrWhiteSpace() && returnDocuments && !query.Schema.IsNullOrEmpty())
            {
                //从 Shcema 中获取TypeName
                var schema = JObject.Parse(query.Schema);
                if (schema != null && schema.ContainsKey("type"))
                {
                    var type = schema["type"].ToString();
                    if (type.StartsWith("ContentItem/", StringComparison.OrdinalIgnoreCase))
                    {
                        contentTypeName = type.Remove(0, 12);
                    }
                }
                return null;
            }
            var fields = _contentManagementAppService.GetFields(typeName);
            //执行并获取结果
            //TODO: 全部导出，需要移除分页参数
            var result = await _queryManager.ExecuteQueryAsync(query, parameters);
            var resultItems = result.Items.Select(x => (ContentItem)x).ToArray();
            var contentPickerItems = new Dictionary<string, ContentItemIndex>();
            var userPickerItems = new Dictionary<string, UserProfileDIndex>();
            ///处理ContentPicker 和 UserPicker 相关信息
            if (fields.Any(x => x.FieldType == "UserPickerField" || x.FieldType == "ContentPickerField"))
            {
                //填充关联的 内容项或用户ID
                FillRelationItemIds(fields, resultItems, contentPickerItems, userPickerItems);

                //获取ID 后 需要统一从数据库查询
                var contentItems = await _session.QueryIndex<ContentItemIndex>().Where(x => x.ContentItemId.IsIn(contentPickerItems.Keys)).ListAsync();
                //var contentItems = await _session.Query<ContentItem, ContentItemIndex>().Where(x => x.ContentItemId.IsIn(contentPickerItems.Keys)).ListAsync();
                foreach (var item in contentItems)
                {
                    contentPickerItems[item.ContentItemId] = item;
                }
            }

            ///TODO: 遍历 导出 Excel
            foreach (var item in resultItems)
            {
                var contentJson = (JObject)item.Content;
                foreach (var field in fields)
                {
                    if (field.FieldType == "ContentPickerField")
                    {
                        //Path可能不起作用，需要测试
                        var contentId = contentJson.SelectToken(field.KeyPath).ToString();
                        // 比如获取 订单上的客户名称
                        Console.WriteLine(contentPickerItems[contentId].DisplayText);

                    }
                    else if (field.FieldType == "UserPickerField")
                    {
                        //Path可能不起作用，需要测试
                        var userId = contentJson.SelectToken(field.KeyPath).ToString();
                        //获取内容选择项ID
                        if (!userPickerItems.ContainsKey(string.Empty))
                        {
                            userPickerItems.Add(userId, null);
                        }
                    }

                }
            }


            return null;


        }

        private void FillRelationItemIds(List<ContentFieldsMappingDto> fields,
            IEnumerable<ContentItem> results,
            Dictionary<string, ContentItemIndex> contentPickerItems,
            Dictionary<string, UserProfileDIndex> userPickerItems)
        {
            foreach (var contentItem in results)
            {
                //TODO: 参考 OrchardCore相关源码 LuceneQuerySource.ExecuteQueryAsync， \OrchardCore\src\OrchardCore.Modules\OrchardCore.Lucene\Services\LuceneQuerySource.cs
                var contentJson = (JObject)contentItem.Content;
                foreach (var field in fields.Where(x => x.FieldType == "UserPickerField" || x.FieldType == "ContentPickerField"))
                {
                    if (field.FieldType == "ContentPickerField")
                    {
                        //Path可能不起作用，需要测试
                        var contentId = contentJson.SelectToken(field.KeyPath).ToString();
                        //获取内容选择项ID
                        if (!contentPickerItems.ContainsKey(string.Empty))
                        {
                            contentPickerItems.Add(contentId, null);
                        }

                    }
                    else if (field.FieldType == "UserPickerField")
                    {
                        //Path可能不起作用，需要测试
                        var userId = contentJson.SelectToken(field.KeyPath).ToString();
                        //获取内容选择项ID
                        if (!userPickerItems.ContainsKey(string.Empty))
                        {
                            userPickerItems.Add(userId, null);
                        }
                    }
                }
            }
        }
    }
}



