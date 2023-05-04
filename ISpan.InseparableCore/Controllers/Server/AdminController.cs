using Microsoft.AspNetCore.Mvc;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class AdminController : AdminSuperController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
