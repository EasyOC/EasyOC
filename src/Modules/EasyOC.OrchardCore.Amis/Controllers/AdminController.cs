using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyOC.OrchardCore.Amis.Controllers
{
    public class AdminController : Controller
    {



        public IActionResult Index()
        {
            return View();
        }
        [Route("EasyOC.OrchardCore.Amis/amis-editor/index")]
        [AllowAnonymous]
        public IActionResult AmisEditor()
        {
            //return Redirect("~/EasyOC.OrchardCore.Amis/amis-editor/index.html");
            return View();
        }

    }
}
