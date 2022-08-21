using EasyOC.ContentExtentions.AppServices;
using EasyOC.Excel.Models;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using OrchardCore.Apis.GraphQL;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EasyOC.Excel.Controllers;

[Route("api/Excel")]
[ApiController]
[Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
public class ApiController : Controller
{

    private readonly ISchemaFactory _schemaService;
    private readonly GraphQLSettings _settings;
    private readonly IDocumentExecuter _executer;
    private readonly IContentTypeManagementAppService _contentTypeManagementAppService;
    public ApiController(ISchemaFactory schemaService, IOptions<GraphQLSettings> settingsOptions, IDocumentExecuter executer, IContentTypeManagementAppService contentTypeManagementAppService)
    {
        _schemaService = schemaService;
        _settings = settingsOptions.Value;
        _executer = executer;
        _contentTypeManagementAppService = contentTypeManagementAppService;
    }

    [HttpPost]
    [Route("export")]
    public async Task<ActionResult> ExportByGraphqlQuery(ExportByGraphqlQueryOptions options)
    {
        var contentType = _contentTypeManagementAppService.GetTypeDefinition(options.ContentType);

        if (options.ContentType is null)
        {
            throw new ArgumentNullException(nameof(options.ContentType));
        }
        var typeDefFields = _contentTypeManagementAppService.GetFields(options.ContentType);

        var allOfHeaders = typeDefFields.Select(x => x.DisplayName).ToArray();

        if (allOfHeaders.Count() != allOfHeaders.Distinct().Count())
        {
            throw new AppFriendlyException(HttpStatusCode.Conflict,
            "该类型存在重复的字段名称，请修复后重试，或者使用自定义列映射");
        }
        if (options.GraphQLQuery.IndexOf("$pageSize", StringComparison.Ordinal) == -1)
        {
            throw new AppFriendlyException(HttpStatusCode.BadRequest,
            "$pageSize参数不存在，请检查查询语句是否正确");
        }




        IWorkbook workbook = new XSSFWorkbook();
        ISheet excelSheet = workbook.CreateSheet(contentType.DisplayName.Substring(0, 30));
        IRow fieldRow = excelSheet.CreateRow(0);
        IRow row = excelSheet.CreateRow(1);
        ICellStyle headerStyle = workbook.CreateCellStyle();
        headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
        headerStyle.FillPattern = FillPattern.SolidForeground;
        var font = workbook.CreateFont();
        font.IsBold = true;
        headerStyle.SetFont(font);
        //TODO: Filter and order typeDefFields , also can filter by user permissions
        for (int i = 0; i < typeDefFields.Count(); i++)
        {
            fieldRow.CreateCell(i).SetCellValue(typeDefFields[i].GraphqlValuePath);
            var headerCel = row.CreateCell(i);
            headerCel.CellStyle = headerStyle;
            headerCel.SetCellValue(typeDefFields[i].DisplayName);
        }
        fieldRow.Hidden = true;

        var pageSize = 0;
        var resultList = await ExecutionResult(options);

        while (resultList != null && resultList.Count > 0)
        {
            foreach (var dataRow in resultList)
            {
                IRow excelRow = excelSheet.CreateRow(excelSheet.LastRowNum + 1);
                var cellIndex = 0;
                foreach (var field in typeDefFields)
                {
                    var cell = excelRow.CreateCell(cellIndex++);
                    var value = dataRow.SelectToken(field.GraphqlValuePath);
                    if (value != null)
                    {
                        cell.SetCellValue(value.ToString());
                    }
                }
            }
            options.QueryParams["pageSize"] = pageSize += 100;
            resultList = await ExecutionResult(options);
        }



        excelSheet.SetAutoFilter(new CellRangeAddress(1, 1, 0, typeDefFields.Count() - 2));//首行筛选
        excelSheet.CreateFreezePane(0, 2);//首行冻结

        byte[] bytes;
        using (var stream = new MemoryStream())
        {
            workbook.Write(stream);
            bytes = stream.ToArray();
        }
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{contentType.DisplayName}.xlsx");

    }
    private async Task<JArray> ExecutionResult(ExportByGraphqlQueryOptions options)
    {
        var context = Request.HttpContext;
        var schema = await _schemaService.GetSchemaAsync();

        // GraphQLRequest request = null;
        var dataLoaderDocumentListener = context.RequestServices.GetRequiredService<IDocumentExecutionListener>();
        var result = await _executer.ExecuteAsync(_ =>
        {
            _.Schema = schema;
            _.Query = options.GraphQLQuery;
            _.Inputs = options.QueryParams.ToInputs();
            _.UserContext = _settings.BuildUserContext?.Invoke(context);
            _.ExposeExceptions = _settings.ExposeExceptions;
            _.ValidationRules = DocumentValidator.CoreRules()
                .Concat(context.RequestServices.GetServices<IValidationRule>());
            _.ComplexityConfiguration = new ComplexityConfiguration
            {
                MaxDepth = _settings.MaxDepth, MaxComplexity = _settings.MaxComplexity, FieldImpact = _settings.FieldImpact
            };
            _.Listeners.Add(dataLoaderDocumentListener);
        });
        ;
        var jResult = JObject.FromObject(result.Data);
        return jResult.SelectToken(options.ContentType.ToCamelCase()) as JArray;
    }
    // [HttpPost]
    // [Route("import")]
    // public ActionResult Import(ImportOptions options)
    // {
    //     return View();
    // }

}
