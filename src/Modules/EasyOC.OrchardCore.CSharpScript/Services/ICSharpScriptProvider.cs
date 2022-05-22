using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.CSharpScript.Services;

public interface ICSharpScriptProvider
{
    Task<AssemblyCSharpBuilder> GetAssemblyCSharpBuilderAsync(bool useGlobalSharedBuilder = true);
    Type GetIndexType(string fullTypeName);
    Task<Type> CreateTypeAsync(string fullName, string cSharpScripts, HashSet<string>? usings = default);
    Task<Type> GetOrCreateAsync(string fullName, string cSharpScripts, HashSet<string>? usings = default);
}
