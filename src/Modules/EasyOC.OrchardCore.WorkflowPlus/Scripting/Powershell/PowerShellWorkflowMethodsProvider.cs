using EasyOC.OrchardCore.WorkflowPlus.Scripting.Powershell;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.WorkflowPlus.Scripting
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



