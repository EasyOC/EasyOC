using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;
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
                    Method = serviceProvider => (Func<string, object>)((command) => {
                        var freeSql= serviceProvider.GetRequiredService<IFreeSql>();
                          var result= freeSql.Ado.Query<object>(command).ToArray();
                        return JArray.FromObject(result);
                    })
                },
               new GlobalMethod
                {
                    Name = "sqlrow",
                    Method = serviceProvider => (Func<string, object>)((command) =>{
                         var freeSql= serviceProvider.GetRequiredService<IFreeSql>();
                        var result=freeSql.Ado.Query<object>(command).FirstOrDefault();
                        return JObject.FromObject(result);
                    })
                },
               new GlobalMethod
                {
                    Name = "sqlScalar",
                    Method = serviceProvider =>(Func<string, object>)((command) => {

                     var freeSql= serviceProvider.GetRequiredService<IFreeSql>();
                        return freeSql.Ado.ExecuteScalar(command);
                    })
                },
               new GlobalMethod
                {
                    Name = "sqlNonQuery",
                    Method = serviceProvider => (Func<string, object>)((command) =>

                    {
                      var freeSql= serviceProvider.GetRequiredService<IFreeSql>();
                         return freeSql.Ado.ExecuteNonQuery(command);
                    })
                }
            };
        }



    }
}



