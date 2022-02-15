using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrchardCore.RelationDb.Controllers
{
    public class AdminController : Controller
    {



        public IActionResult Index()
        {
            return View();
        }
        [Route("JZSoft.OrchardCore.Amis/amis-editor/index")]
        [AllowAnonymous]
        public IActionResult AmisEditor()
        {
            //return Redirect("~/JZSoft.OrchardCore.Amis/amis-editor/index.html");
            return View();
        }

    }
}
