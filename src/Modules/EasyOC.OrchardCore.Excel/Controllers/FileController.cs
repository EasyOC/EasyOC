using Microsoft.AspNetCore.Mvc;

namespace EasyOC.OrchardCore.Excel.Controllers
{
    public class FileController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult UploadFile()
        {

            return View();
        }
    }
}



