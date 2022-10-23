using EasyOC.Core.Application;
using EasyOC.Core.DtoModels;
using EasyOC.DynamicWebApi;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.Excel.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.Excel.Services
{
    [DynamicWebApi]
    public class ExcelAppService : AppServiceBase, IExcelAppService
    {
        private readonly IContentManager _contentManager;

        public ExcelAppService(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }
        [IgnoreWebApiMethod]
        public List<object> Evaluate(string script, object data)
        {
            var result = JsEngine
                   .Execute("var excelTable = " + JsonSerializer.Serialize(data) + ";")
                   .Evaluate(script)?.ToObject();
            if (result == null)
            {
                return null;
            }
            var objArray = (object[])result;
            var dynamicArray = objArray.Select(x => x);
            return dynamicArray.ToList();
        }

        [IgnoreWebApiMethod]
        public DataTable GetExcelDataFromConfigFromStream(Stream stream, string rowFilterExpression = "")
        {
            return GetExcelDataFromConfigFromStream(stream, new ReadExcelOptions
            {
                FilterExpression = rowFilterExpression
            });
        }

        [IgnoreWebApiMethod]
        public DataTable GetExcelDataFromConfigFromStream(Stream stream, ReadExcelOptions options)
        {
            var table = MiniExcel.QueryAsDataTable(stream, useHeaderRow: true,sheetName: options.SheetName, startCell: options.StartAddress??"A1");

            foreach (DataColumn item in table.Columns)
            {
                item.ColumnName = item.ColumnName.Trim().Replace(' ', '_').Replace(".", "_");
            }
            if (options.FilterExpression is not null)
            {
                //var dataTable = table.Select("Shipping_point='2049' and Created_Date>='2021-08-01'")
                var temp = table.Select(options.FilterExpression);
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

    //[IgnoreWebApiMethod]
    //public DataTable GetExcelDataFromConfigFromStream(Stream stream, ReadExcelOptions options)
    //{
    //    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    //    //文档：https://github.com/ExcelDataReader/ExcelDataReader
    //    using (var reader = ExcelReaderFactory.CreateReader(stream))
    //    {
    //        var rowIndex = 0;
    //        var startRowNum = options.StartRowNum ?? 1;
    //        var startColNum = options.StartColNum ?? 1;
    //        var rowFilterExpression = options.FilterExpression;

    //        // 2. Use the AsDataSet extension method
    //        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
    //        {
    //            // Gets or sets a value indicating whether to set the DataColumn.DataType
    //            // property in a second pass.
    //            UseColumnDataType = true,

    //            // Gets or sets a callback to determine whether to include the current sheet
    //            // in the DataSet. Called once per sheet before ConfigureDataTable.
    //            FilterSheet = (tableReader, sheetIndex) =>
    //            {
    //                if (options.SheetName is not null)
    //                {
    //                    return tableReader.Name == options.SheetName;
    //                }
    //                if (options.SheetIndex is not null)
    //                {
    //                    return sheetIndex == options.SheetIndex;
    //                }
    //                return true;
    //            }
    //            ,

    //            // Gets or sets a callback to obtain configuration options for a DataTable.
    //            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
    //            {
    //                EmptyColumnNamePrefix = "Column",
    //                // Gets or sets a value indicating whether to use a row from the
    //                // data as column names.
    //                UseHeaderRow = true,
    //                //// Gets or sets a callback to determine which row is the header row.
    //                //// Only called when UseHeaderRow = true.
    //                ReadHeaderRow = (rowReader) =>
    //                {

    //                    // F.ex skip the first row and use the 2nd row as column headers:

    //                },
    //                //// Gets or sets a callback to determine whether to include the
    //                //// current row in the DataTable.
    //                FilterRow = (rowReader) =>
    //                {
    //                    rowIndex++;
    //                    return (startRowNum <= rowIndex);
    //                },
    //                //// Gets or sets a callback to determine whether to include the specific
    //                //// column in the DataTable. Called once per column after reading the
    //                //// headers.
    //                FilterColumn = (rowReader, columnIndex) =>
    //                {
    //                    return (startColNum <= (columnIndex + 1));
    //                }
    //            }
    //        });
    //        // The result of each spreadsheet is in result.Tables
    //        var table = result.Tables[0];
    //        foreach (DataColumn item in table.Columns)
    //        {
    //            item.ColumnName = item.ColumnName.Trim().Replace(' ', '_').Replace(".", "_");
    //        }
    //        if (!string.IsNullOrWhiteSpace(rowFilterExpression))
    //        {
    //            //var dataTable = table.Select("Shipping_point='2049' and Created_Date>='2021-08-01'")
    //            var temp = table.Select(rowFilterExpression);
    //            if (temp?.Length > 0)
    //            {
    //                table = temp.CopyToDataTable();
    //            }
    //            else
    //            {
    //                table = null;
    //            }
    //        }
    //        if (Logger.IsEnabled(LogLevel.Debug))
    //        {
    //            Logger.LogDebug("Excel 读取完成:{result}", table);
    //        }
    //        return table;
    //    }
    //}

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
        using (var fs = File.Open(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            var data = GetExcelDataFromConfigFromStream(fs, rowFilterExpression);
            return Evaluate(settings.FieldsMappingConfig.Text, data);
        }
    }

    [IgnoreWebApiMethod]
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



