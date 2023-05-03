using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjMvcCoreDemo.Models;
using System.Text.Json;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class AdminHomeController : AdminSuperController
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
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
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
                ModelState.AddModelError("Email", "Email錯誤，請檢查您的輸入並重試");
            }

            if (ModelState.IsValid) // 驗證通過
            {
                if (administrator != null && CPasswordHelper.VerifyPassword(model.Password, administrator.FPasswordHash, administrator.FPasswordSalt))
                {
                    string json = JsonSerializer.Serialize(administrator);
                    HttpContext.Session.SetString(CDictionary.SK_LOGINED_USER, json);
                    return RedirectToAction(nameof(AdminMemberController.Index), "AdminMember");
                }
                else
                {
                    ModelState.AddModelError("Password", "密碼錯誤，請重試");
                    return View(model);
                }
            }

            return View(model);
        }



        public IActionResult Index()
        {
            return View();
        }
    }
}
