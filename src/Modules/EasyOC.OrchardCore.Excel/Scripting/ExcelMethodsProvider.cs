using EasyOC.OrchardCore.Excel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC.OrchardCore.Excel.Scripting
{
    public class ExcelMethodsProvider : IGlobalMethodProvider
    {


        public IEnumerable<GlobalMethod> GetMethods()
        {
            return new[] {
                new GlobalMethod{
                  Name = "readExcelFromRequest",
                    Method = serviceProvider => (Func<string,object>)(rowFilterExpression =>{
                        var  httpContextAccessor= serviceProvider.GetRequiredService<IHttpContextAccessor>();
                            var formFiles =  httpContextAccessor.HttpContext.Request.Form.Files;
                            if (formFiles.Count==0)
                            {
                                return null;
                            }
                            else
                            {
                                var  _excelAppService= serviceProvider.GetRequiredService<IExcelAppService>();
                                var file=formFiles.FirstOrDefault();
                                using(var stream= file.OpenReadStream())
                                {
                                    var table= _excelAppService.GetExcelDataFromConfigFromStream(stream,rowFilterExpression);
                                    if (table != null&& table.Rows.Count>0)
                                    {
                                        return JObject.Parse(JsonConvert.SerializeObject(table));

                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                            }
                        })
                }



            };
        }
    }
}



