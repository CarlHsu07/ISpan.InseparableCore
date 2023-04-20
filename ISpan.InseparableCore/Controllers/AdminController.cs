using Microsoft.AspNetCore.Mvc;

namespace ISpan.InseparableCore.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
