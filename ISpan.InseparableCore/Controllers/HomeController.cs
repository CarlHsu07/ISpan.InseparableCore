using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.Models.DAL.Repo;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
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
            TMembers? member = await _context.TMembers.FirstOrDefaultAsync(m => m.FEmail == model.Email);

            if (member == null) // 找不到該會員，即Email錯誤
            {
                ModelState.AddModelError(nameof(CMemberLoginViewModel.Email), "Email錯誤");
            }
            else if (member.FIsEmailVerified == false) // 信箱驗證尚未完成
            {
                ModelState.AddModelError(nameof(CMemberLoginViewModel.Email), "Email尚未驗證，請至信箱收驗證信並進行驗證");
            }
            else if (member.FAccountStatus == 3) // 會員狀態是已註銷
            {
                ModelState.AddModelError(nameof(CMemberLoginViewModel.Email), "帳號已註銷");
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
                    ModelState.AddModelError("Password", "密碼錯誤");
                    return View(model);
                }
            }

            return View(model);
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

            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER)) // 在登入狀態下可查到會員跟文章
            {
                member = member_repo.searchMembers(keyword);

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