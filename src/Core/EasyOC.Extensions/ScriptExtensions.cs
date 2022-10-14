using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using OrchardCore.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC
{
    public static class ScriptExtensions
    {

        public static IScriptingScope CreateScope(this IScriptingManager scriptingManager,
            string perfix, IServiceProvider serviceProvider,
            IEnumerable<GlobalMethod> extraMethods = null,
            IFileProvider fileProvider = null,
            string basePath = null
            )
        {
            var engine = scriptingManager.GetScriptingEngine(perfix);
            var methods = scriptingManager.GlobalMethodProviders.SelectMany(x => x.GetMethods());
            if (extraMethods != null)
            {
                methods = methods.Concat(extraMethods);
            }
            var scope = engine.CreateScope(methods,
                serviceProvider, fileProvider, basePath);
            return scope;
        }
    }
}
