using Microsoft.AspNetCore.Mvc;

namespace ISpan.InseparableCore.Controllers.Server
{
	public class AdminArticleController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
