using OrchardCore.Scripting.JavaScript;
using System.Threading.Tasks;

namespace EasyOC.Scripting.Servicies
{
    public interface IDbAccessableJSScopeBuilder
    {
        Task<JavaScriptScope> CreateScopeAsync();
    }
}