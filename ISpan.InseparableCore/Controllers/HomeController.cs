using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ISpan.InseparableCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InseparableContext _db;
        public HomeController(ILogger<HomeController> logger, InseparableContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            ChomeIndexVM vm = new ChomeIndexVM();
            DateTime today = DateTime.Now.Date;
            vm.showing = _db.TMovies.Where(t => t.FMovieOffDate > today && t.FMovieOnDate < today).OrderByDescending(t => t.FMovieOffDate).Take(6); 
            vm.soon = _db.TMovies.Where(t => t.FMovieOnDate > today).OrderBy(t => t.FMovieOffDate).Take(6); 
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}