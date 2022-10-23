using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace EasyOC.RDBMS.Scripting
{
    public class FreeSqlWorkflowMethodsProvider : IGlobalMethodProvider
    {


        public IEnumerable<GlobalMethod> GetMethods()
        {
            return new[] {
               new GlobalMethod
                {
                    Name = "sqlrows",
                    Method = serviceProvider => (Func<string,ExpandoObject, object>)((command,input) => {
                        var freeSql= serviceProvider.GetRequiredService<IFreeSql>();
                          var result= freeSql.Ado.Query<object>(command,input.ToDictionary()).ToArray();
                        return JArray.FromObject(result);
                    })
                },
               new GlobalMethod
                {
                    Name = "sqlrow",
                    Method = serviceProvider => (Func<string,ExpandoObject, object>)((command,input) => {
                         var freeSql= serviceProvider.GetRequiredService<IFreeSql>();
                        var result=freeSql.Ado.Query<object>(command,input.ToDictionary()).FirstOrDefault();
                        return JObject.FromObject(result);
                    })
                },
               new GlobalMethod
                {
                    Name = "sqlScalar",
                    Method = serviceProvider =>(Func<string,ExpandoObject, object>)((command,input) => {

                     var freeSql= serviceProvider.GetRequiredService<IFreeSql>();
                        return freeSql.Ado.ExecuteScalar(command,input.ToDictionary());
                    })
                },
               new GlobalMethod
                {
                    Name = "sqlNonQuery",
                    Method = serviceProvider => (Func<string,ExpandoObject, object>)((command,input) => 
                    {
                      var freeSql= serviceProvider.GetRequiredService<IFreeSql>();
                         return freeSql.Ado.ExecuteNonQuery(command,input.ToDictionary());
                    })
                }
            };
        }
    }
}



