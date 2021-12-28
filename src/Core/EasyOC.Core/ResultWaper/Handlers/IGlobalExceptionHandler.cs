using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace EasyOC.Core.ResultWaper.Handlers
{
    public interface IGlobalExceptionHandler
    {
        Task OnExceptionAsync(ExceptionContext context);
    }
}
