using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class AdminHomeController : Controller
    {
        private readonly InseparableContext _context;
        IWebHostEnvironment _enviro;

        public AdminHomeController(InseparableContext context, IWebHostEnvironment enviro)
        {
            _context = context;
            _enviro = enviro;
        }

        // GET: AdminHome/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_ADMINISTRATOR))
            {
                return RedirectToAction(nameof(AdminMemberController.Index), "AdminMember");
            }

            return View();
        }

        // POST: AdminHome/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] CAdminLoginViewModel model)
        {
            TAdministrators administrator = await _context.TAdministrators.FirstOrDefaultAsync(m => m.FEmail == model.Email);

            if (administrator == null) // 找不到該會員，即Email錯誤
            {
                ModelState.AddModelError("Email", "Email錯誤，帳號不存在");
            }

            if (ModelState.IsValid) // 驗證通過
            {
                if (administrator != null && CPasswordHelper.VerifyPassword(model.Password, administrator.FPasswordHash, administrator.FPasswordSalt))
                {
                    string json = JsonSerializer.Serialize(administrator);
                    HttpContext.Session.SetString(CDictionary.SK_LOGINED_ADMINISTRATOR, json);
                    return RedirectToAction(nameof(AdminMemberController.Index), "AdminMember");
                }
                else
                {
                    ModelState.AddModelError("Password", "密碼錯誤");
                    return View(model);
                }
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_ADMINISTRATOR))
            {
                HttpContext.Session.Remove(CDictionary.SK_LOGINED_ADMINISTRATOR);
                return RedirectToAction(nameof(AdminHomeController.Login), "AdminHome");
            }

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
