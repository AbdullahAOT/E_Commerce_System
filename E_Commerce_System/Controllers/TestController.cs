using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Hello()
        {
            return Content("Hello from TestController");
        }
    }
}
