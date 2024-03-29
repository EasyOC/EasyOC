﻿using EasyOC.Core.DtoModels;
using EasyOC.Excel.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace EasyOC.Excel.Services
{
    public interface IExcelAppService
    {
        List<object> Evaluate(string script, object data);
        Task<IEnumerable<SelectListItem>> GetAllExcelSettingsAsync();
        DataTable GetExcelDataFromConfigFromStream(Stream stream, string rowFilterExpression = "");
        DataTable GetExcelDataFromConfigFromStream(Stream stream, ReadExcelOptions options);
        Task<DataTable> GetExcelDataFromConfigAsync(string fileFullPath, string rowFilterExpression = "");
        Task<ContentItemDto> GetExcelSettingsAsync(string displayText);
        Task<List<object>> GetProcessedDataAsync(string fileFullPath, string configDocumentId, string rowFilterExpression = "");

    }
}


