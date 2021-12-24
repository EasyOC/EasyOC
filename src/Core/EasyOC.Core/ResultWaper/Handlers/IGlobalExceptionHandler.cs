using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.Core.ResultWaper.Handlers
{
    public interface IGlobalExceptionHandler
    {
        Task OnExceptionAsync(ExceptionContext context);
    }
}
