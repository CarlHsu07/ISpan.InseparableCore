using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjMvcCoreDemo.Models;
using System.Diagnostics;
using System.Text.Json;

namespace ISpan.InseparableCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InseparableContext _context;
        IWebHostEnvironment _enviro;

        public HomeController(ILogger<HomeController> logger, InseparableContext context, IWebHostEnvironment enviro)
        {
            _logger = logger;
            _context = context;
            _enviro = enviro;
        }
   
        public IActionResult Index()
        {
            ChomeIndexVM vm = new ChomeIndexVM();
            DateTime today = DateTime.Now.Date;
            vm.showing = _context.TMovies.Take(6); //Where(t => t.FMovieOffDate < today).OrderByDescending(t => t.FMovieOffDate).
            vm.soon = _context.TMovies.Take(6); //Where(t => t.FMovieOffDate > today).OrderBy(t => t.FMovieOffDate).
            return View(vm);
        }

        // GET: Home/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                var serializedTMembers = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                var member = JsonSerializer.Deserialize<TMembers>(serializedTMembers);
                return RedirectToAction(nameof(MemberController.Index), "Member", new { id = member.FId });
            }

            return View();
        }

        // POST: Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] CMemberLoginViewModel model)
        {
            TMembers member = _context.TMembers.FirstOrDefault(m => m.FEmail == model.Email);

            if (member == null) // 找不到該會員，即Email錯誤
            {
                ModelState.AddModelError("Email", "Email錯誤，請檢查您的輸入並重試");
            }

            if (ModelState.IsValid) // 驗證
            {
                if (member != null && CPasswordHelper.VerifyPassword(model.Password, member.FPasswordHash, member.FPasswordSalt))
                {
                    string json = JsonSerializer.Serialize(member);
                    HttpContext.Session.SetString(CDictionary.SK_LOGINED_USER, json);
                    return RedirectToAction(nameof(MemberController.Index), "Member", new { id = member.FId });
                }
                else
                {
                    ModelState.AddModelError("Password", "密碼錯誤，請重試");
                    return View(model);
                }
            }

            return View(model);
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