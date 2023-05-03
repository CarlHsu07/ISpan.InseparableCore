using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.ViewModels;
using System.Text;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.Models.BLL;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class AdminMemberController : AdminSuperController
    {
        private readonly InseparableContext _context;

        public AdminMemberController(InseparableContext context)
        {
            _context = context;
        }

        // GET: AdminMember
        public async Task<IActionResult> Index(CQueryKeywordViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.txtKeyword)) // 搜尋關鍵字是空的
            {
                var inseparableContext = _context.TMembers
                .Include(t => t.FAccountStatusNavigation)
                .Include(t => t.FArea)
                .Include(t => t.FGender);

                return View(await inseparableContext.ToListAsync());
            }
            else // 搜尋關鍵字不是空的
            {
                var inseparableContext = _context.TMembers
                    .Where(m =>
                    m.FFirstName.Contains(vm.txtKeyword) ||
                    m.FLastName.Contains(vm.txtKeyword) ||
                    m.FEmail.Contains(vm.txtKeyword) ||
                    m.FMemberId.Contains(vm.txtKeyword)
                    )
                    .Include(t => t.FAccountStatusNavigation)
                    .Include(t => t.FArea)
                    .Include(t => t.FGender);

                return View(await inseparableContext.ToListAsync());
            }
        }

        // GET: AdminMember/Details/5
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

        // GET: AdminMember/Register
        public IActionResult Create()
        {
            // todo 居住區域選單，要改成縣市跟區域，先選縣市再顯示區域
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus");
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName");
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType");
            return View();
        }

        // POST: AdminMember/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FLastName,FFirstName,FEmail,FPasswordHash,FDateOfBirth,FGenderId,FCellphone,FAddress,FAreaZipCode,FPhotoPath,FIntroduction,FAccountStatus,FTotalMemberPoint")] TMembers MemberIn)
        {
            if (ModelState.IsValid)
            {
                MemberService memberService = new MemberService(_context);

                // 產生會員ID
                MemberIn.FMemberId = memberService.GenerateMemberId();

                // 產生會員註冊時間
                MemberIn.FSignUpTime = memberService.GenerateSignUpTime();

                // 產生會員點數
                if (MemberIn.FTotalMemberPoint == null)
                {
                    MemberIn.FTotalMemberPoint = 0;
                }

                // 加密會員密碼
                #region
                string password = MemberIn.FPasswordHash; // 要加密的密碼

                // 產生鹽值
                byte[] salt = CPasswordHelper.GenerateSalt();

                // 將密碼與鹽值結合後進行加密
                byte[] hashedPassword = CPasswordHelper.HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), salt);

                // 將鹽值與加密後的密碼轉換成 Base64 字串儲存
                MemberIn.FPasswordSalt = Convert.ToBase64String(salt);
                MemberIn.FPasswordHash = Convert.ToBase64String(hashedPassword);
                #endregion

                _context.Add(MemberIn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", MemberIn.FAccountStatus);
            // todo 改成顯示縣市；區域用ajax處理
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", MemberIn.FAreaId);
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", MemberIn.FGenderId);
            return View(MemberIn);
        }

        // GET: AdminMember/Edit/5
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

        // POST: AdminMember/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FMemberId,FLastName,FFirstName,FEmail,FPasswordHash,FPasswordSalt,FDateOfBirth,FGenderId,FCellphone,FAddress,FAreaZipCode,FPhotoPath,FIntroduction,FAccountStatus,FTotalMemberPoint")] TMembers tMembers)
        {
            if (id != tMembers.FId) // todo 有問題，tMembers.FId為0
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

        // GET: AdminMember/Delete/5
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

        // POST: AdminMember/Delete/5
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

    }
}
