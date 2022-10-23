using EasyOC.Excel.Models;
using EasyOC.Excel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace EasyOC.Excel.Scripting
{
    public class ExcelMethodsProvider : IGlobalMethodProvider
    {


        public IEnumerable<GlobalMethod> GetMethods()
        {
            return new[]
            {
                new GlobalMethod
                {
                    Name = "readExcel",
                    //upload?,formKey|filepath,configObject
                    Method = serviceProvider => (Func<string,bool?,string, object>)((fileKeyOrPath,fromUpload,configObject) =>
                    {
                        var readOption= string.IsNullOrEmpty(configObject)? new ReadExcelOptions(): JsonConvert.DeserializeObject<ReadExcelOptions>(configObject) ;
                        DataTable dataTable=null;
                         var _excelAppService = serviceProvider.GetRequiredService<IExcelAppService>();
                        if (!fromUpload.HasValue||fromUpload.Value)
                        {
                            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                            var formFiles = httpContextAccessor?.HttpContext?.Request.Form.Files;
                            if (formFiles is { Count: 0 })
                            {
                                return null;
                            }
                            var file =fileKeyOrPath is not null ?
                                formFiles[fileKeyOrPath] :
                                formFiles.FirstOrDefault();
                            if (file != null)
                            {
                                using var stream = file.OpenReadStream();
                                dataTable = _excelAppService.GetExcelDataFromConfigFromStream(stream, readOption);

                            }
                        }
                        else
                        {
                            using var stream = File.Open(fileKeyOrPath,FileMode.Open,FileAccess.Read, FileShare.ReadWrite);
                            dataTable = _excelAppService.GetExcelDataFromConfigFromStream(stream, readOption);
                        }

                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            return JToken.FromObject(dataTable);
                        }
                        return null;
                    })
                }
            };
        }

    }
}
