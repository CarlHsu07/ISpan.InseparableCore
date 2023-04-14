using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models;
using System.Reflection;
using System.Text;
using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;

namespace ISpan.InseparableCore.Controllers
{
    public class MemberController : Controller
    {
        private readonly InseparableContext _context;

        public MemberController(InseparableContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            var inseparableContext = _context.TMembers.Include(t => t.FAccountStatusNavigation).Include(t => t.FArea).Include(t => t.FGender);
            return View(await inseparableContext.ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var tMembers = await _context.TMembers
                .Include(t => t.FAccountStatusNavigation)
                .Include(t => t.FArea)
                .Include(t => t.FGender)
                .FirstOrDefaultAsync(m => m.FId == id);
            if (tMembers == null)
            {
                return NotFound();
            }

            return View(tMembers);
        }

        // GET: Members/Register
        public IActionResult Register()
        {
            // 縣市
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName");

            //ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName");
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType");
            return View();
        }

        // POST: Members/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(/*[Bind("FLastName,FFirstName,FEmail,FPasswordHash,FDateOfBirth,FGenderId,FCellphone,FAddress,FAreaZipCode")]*/ CRegisterViewModel MemberIn)
        {
            TMembers newMember = new TMembers();

            if (ModelState.IsValid)
            {
                MemberService memberService = new MemberService(_context);

                // 產生會員ID
                newMember.FMemberId = memberService.GenerateMemberId();

                // 產生會員註冊時間
                newMember.FSignUpTime = memberService.GenerateSignUpTime();

                // 產生會員點數
                if (newMember.FTotalMemberPoint == null)
                {
                    newMember.FTotalMemberPoint = 0;
                }

                newMember.FLastName = MemberIn.LastName;
                newMember.FFirstName = MemberIn.FirstName;
                newMember.FEmail = MemberIn.Email;
                newMember.FDateOfBirth = MemberIn.DateOfBirth;
                newMember.FGenderId = MemberIn.GenderId;
                newMember.FAreaId = MemberIn.Area;

                // 加密會員密碼
                #region
                string password = MemberIn.Password; // 要加密的密碼

                // 產生鹽值
                byte[] salt = CPasswordHelper.GenerateSalt();

                // 將密碼與鹽值結合後進行加密
                byte[] hashedPassword = CPasswordHelper.HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), salt);

                // 將鹽值與加密後的密碼轉換成 Base64 字串儲存
                newMember.FPasswordSalt = Convert.ToBase64String(salt);
                newMember.FPasswordHash = Convert.ToBase64String(hashedPassword);
                #endregion



                _context.Add(newMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", newMember.FAccountStatus);
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", newMember.FAreaId);
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", newMember.FGenderId);
            return View(MemberIn);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var tMembers = await _context.TMembers.FindAsync(id);
            if (tMembers == null)
            {
                return NotFound();
            }
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", tMembers.FAccountStatus);
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", tMembers.FAreaId);
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", tMembers.FGenderId);
            return View(tMembers);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FId,FMemberId,FLastName,FFirstName,FEmail,FPasswordHash,FPasswordSalt,FDateOfBirth,FGenderId,FCellphone,FAddress,FAreaZipCode,FPhotoPath,FIntroduction,FAccountStatus,FTotalMemberPoint,FSignUpTime")] TMembers tMembers)
        {
            if (id != tMembers.FId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tMembers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TMembersExists(tMembers.FId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", tMembers.FAccountStatus);
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", tMembers.FAreaId);
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", tMembers.FGenderId);
            return View(tMembers);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var tMembers = await _context.TMembers
                .Include(t => t.FAccountStatusNavigation)
                .Include(t => t.FArea)
                .Include(t => t.FGender)
                .FirstOrDefaultAsync(m => m.FId == id);
            if (tMembers == null)
            {
                return NotFound();
            }

            return View(tMembers);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TMembers == null)
            {
                return Problem("Entity set 'InseparableContext.TMembers'  is null.");
            }
            var tMembers = await _context.TMembers.FindAsync(id);
            if (tMembers != null)
            {
                _context.TMembers.Remove(tMembers);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TMembersExists(int id)
        {
          return (_context.TMembers?.Any(e => e.FId == id)).GetValueOrDefault();
        }

        // 取得指定縣市的區域(鄉鎮市區)
        [HttpPost]
        public IActionResult GetAreas(int cityId)
        {
            // 根據縣市值，查詢對應的區域資料
            var areas = _context.TAreas
                .Where(a => a.FCityId == cityId)
                .Select(a => new
                {
                    zipCode = a.FZipCode,
                    areaName = a.FAreaName
                })
                .ToList();

            // 將區域資料轉換成 JSON 格式回傳給前端
            return Json(areas);
        }
    }
}
