using ExcelDataReader;
using EasyOC.Core.Application;
using EasyOC.Core.Models;
using EasyOC.OrchardCore.Excel.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using Panda.DynamicWebApi;
using Panda.DynamicWebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.OrchardCore.Excel.Services
{
    [DynamicWebApi]
    public class ExcelAppService : AppServcieBase, IDynamicWebApi, IExcelAppService
    {
        private readonly IContentManager _contentManager;

        public ExcelAppService(IContentManager contentManager) 
        {
            _contentManager = contentManager;
        }
        [NonDynamicMethod]
        public List<object> Evaluate(string script, object data)
        {

            var result = JSEngine
                   .Execute("var excelTable = " + JsonSerializer.Serialize(data) + ";")
                   .Evaluate(script)?.ToObject();
            if (result != null)
            {
                var objArray = (object[])result;
                var dynamicArray = objArray.Select(x => x);
                return dynamicArray.ToList();
            }
            return null;
        }

        [NonDynamicMethod]
        public DataTable GetExcelDataFromConfigFromStream(Stream stream, string rowFilterExpression = "")
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //文档：https://github.com/ExcelDataReader/ExcelDataReader
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {

                // 2. Use the AsDataSet extension method
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    // Gets or sets a value indicating whether to set the DataColumn.DataType 
                    // property in a second pass.
                    UseColumnDataType = true,

                    // Gets or sets a callback to determine whether to include the current sheet
                    // in the DataSet. Called once per sheet before ConfigureDataTable.
                    //FilterSheet = (tableReader, sheetIndex) =>
                    //    tableReader.Name == settings.SheetName.Text
                    //,

                    // Gets or sets a callback to obtain configuration options for a DataTable. 
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        // Gets or sets a value indicating whether to use a row from the 
                        // data as column names.
                        UseHeaderRow = true,
                        //// Gets or sets a callback to determine which row is the header row. 
                        //// Only called when UseHeaderRow = true.
                        //ReadHeaderRow = (rowReader) =>
                        //{
                        //    // F.ex skip the first row and use the 2nd row as column headers:
                        //    rowReader.Read();
                        //},
                        //// Gets or sets a callback to determine whether to include the 
                        //// current row in the DataTable.
                        //FilterRow = (rowReader) =>
                        //{
                        //    return true;
                        //},

                        //// Gets or sets a callback to determine whether to include the specific
                        //// column in the DataTable. Called once per column after reading the 
                        //// headers.
                        //FilterColumn = (rowReader, columnIndex) =>
                        //{
                        //    return true;
                        //}
                    }
                });
                // The result of each spreadsheet is in result.Tables
                var table = result.Tables[0];
                foreach (DataColumn item in table.Columns)
                {
                    item.ColumnName = item.ColumnName.Trim().Replace(' ', '_').Replace(".", "_");
                }
                if (!string.IsNullOrWhiteSpace(rowFilterExpression))
                {
                    //var dataTable = table.Select("Shipping_point='2049' and Created_Date>='2021-08-01'") 
                    var temp = table.Select(rowFilterExpression);
                    if (temp?.Length > 0)
                    {
                        table = temp.CopyToDataTable();
                    }
                    else
                    {
                        table = null;
                    }
                }
                if (Logger.IsEnabled(LogLevel.Debug))
                {
                    Logger.LogDebug("Excel 读取完成:{result}", table);
                }
                return table;
            }
        }

        public async Task<DataTable> GetExcelDataFromConfigAsync(string fileFullPath, string rowFilterExpression = "")
        {
            using (var fs = File.OpenRead(fileFullPath))
            {
                return await Task.FromResult(GetExcelDataFromConfigFromStream(fs, rowFilterExpression));
            }
        }

        public async Task<List<object>> GetProcessedDataAsync(string fileFullPath, string configDocumentId, string rowFilterExpression = "")
        {
            var configItem = await _contentManager.GetAsync(configDocumentId);
            var settings = configItem.As<ImportExcelSettings>();
            using (var fs = File.OpenRead(fileFullPath))
            {
                var data = GetExcelDataFromConfigFromStream(fs, rowFilterExpression);
                return Evaluate(settings.FieldsMappingConfig.Text, data);
            }
        }
        [NonDynamicMethod]
        public async Task<ContentItemDto> GetExcelSettingsAsync(string displayText)
        {
            var contentItem = await YesSession.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.ContentType == "ImportExcelSettings" && (x.Published && x.Latest) && x.DisplayText == displayText).FirstOrDefaultAsync();
            return contentItem.ToDto<ContentItemDto>();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllExcelSettingsAsync()
        {
            var ls = await YesSession.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.ContentType == "ImportExcelSettings" && (x.Published || x.Latest)).ListAsync();
            return ls.Select(x => new SelectListItem { Text = x.DisplayText, Value = x.ContentItemId });
        }


    }
}



