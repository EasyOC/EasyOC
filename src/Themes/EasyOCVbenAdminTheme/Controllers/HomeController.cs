using Microsoft.AspNetCore.Mvc;

namespace EasyOCVbenAdminTheme.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
