using Microsoft.AspNetCore.Mvc;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
