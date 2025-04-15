using Microsoft.AspNetCore.Mvc;

namespace campusjobv2.Controllers
{
    public class HomepageController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Homepage";
            return View();
        }
    }
}
