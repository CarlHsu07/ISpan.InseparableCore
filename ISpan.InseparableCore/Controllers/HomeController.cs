using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.Models.DAL.Repo;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using prjMvcCoreDemo.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISpan.InseparableCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InseparableContext _context;
        private readonly MovieRepository movie_repo;
        private readonly CinemaRepository cinema_repo;
        private readonly ArticleRepository article_repo;
        private readonly MemberRepository member_repo;
        IWebHostEnvironment _enviro;

        public HomeController(ILogger<HomeController> logger, InseparableContext context, IWebHostEnvironment enviro)
        {
            _logger = logger;
            _context = context;
            _enviro = enviro;
            movie_repo = new MovieRepository(context, null);
            cinema_repo = new CinemaRepository(context);
            article_repo = new ArticleRepository(context);
            member_repo = new MemberRepository(context);
        }

        public IActionResult Index()
        {
            ChomeIndexVM vm = new ChomeIndexVM();

            vm.showing = movie_repo.Showing(); 
            vm.soon = movie_repo.Soon();
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

            if (ModelState.IsValid) // 驗證通過
            {
                if (member != null && CPasswordHelper.VerifyPassword(model.Password, member.FPasswordHash, member.FPasswordSalt))
                {
                    string json = JsonSerializer.Serialize(member);
                    HttpContext.Session.SetString(CDictionary.SK_LOGINED_USER, json);
                    return RedirectToAction(nameof(MemberController.Index), "Member");
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
        [HttpPost]
        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return Ok(null);

            var movie = movie_repo.Movie(keyword);
            var cinema = cinema_repo.Cinema(keyword);
            IEnumerable<CMemberVM> member = Enumerable.Empty<CMemberVM>();
            IEnumerable<ArticleVm> articles = Enumerable.Empty<ArticleVm>();
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                member = member_repo.members(keyword);

                articles = article_repo.Articles(keyword);
            }
            
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string moviejson = JsonSerializer.Serialize(movie, options);
            
            JsonSerializerOptions cinemaoptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string cinemajson = JsonSerializer.Serialize(cinema, cinemaoptions);
            JsonSerializerOptions memberoptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string memberjson = JsonSerializer.Serialize(member, memberoptions);
            JsonSerializerOptions articlesoptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string articlesjson = JsonSerializer.Serialize(articles, articlesoptions);
            return Ok(new
            {
                cinema = cinemajson,
                movie = moviejson,
                member = memberjson,
                articles = articlesjson,
            }.ToJson());
        }
    }
}