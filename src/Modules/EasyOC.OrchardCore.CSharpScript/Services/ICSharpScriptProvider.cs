using System;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.CSharpScript.Services;

public interface ICSharpScriptProvider
{
    Task<AssemblyCSharpBuilder> GetAssemblyCSharpBuilderAsync(bool useGlobalSharedBuilder = true);

    Task<Type> CreateTypeAsync(string fullName, string cSharpScripts);
    Task<Type> GetOrCreateAsync(string fullName, string cSharpScripts);
}
