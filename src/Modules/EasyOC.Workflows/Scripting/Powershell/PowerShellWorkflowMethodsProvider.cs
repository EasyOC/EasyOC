using EasyOC.Workflows.Scripting.Powershell;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;

namespace EasyOC.Workflows.Scripting
{
    public class PowerShellWorkflowMethodsProvider : IGlobalMethodProvider
    {
        public IEnumerable<GlobalMethod> GetMethods()
        {
            return new[] {
               new GlobalMethod
                {
                    Name = "powershell",
                    Method = serviceProvider => (Func<string, object>)(PowerShellExecuter.Excute)
                }
            };
        }
    }
}



