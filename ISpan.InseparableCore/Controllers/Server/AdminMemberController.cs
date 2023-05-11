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
using System.Reflection;
using Microsoft.Extensions.Options;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class AdminMemberController : AdminSuperController
    {
        private readonly InseparableContext _context;
        private readonly ApiKeys _key;

        public AdminMemberController(InseparableContext context, IOptions<ApiKeys> key)
        {
            _context = context;
            _key = key.Value;
        }

        // GET: AdminMember
        public async Task<IActionResult> Index(CQueryKeywordViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.txtKeyword)) // 搜尋關鍵字是空的，顯示全部資料
            {
                var members = _context.TMembers
                .Include(m => m.FAccountStatusNavigation)
                .Include(m => m.FArea)
                .Include(m => m.FGender)
                .OrderByDescending(m => m.FId);

                return View(await members.ToListAsync());
            }
            else // 搜尋關鍵字不是空的
            {
                var members = _context.TMembers
                    .Where(m =>
                    m.FFirstName.Contains(vm.txtKeyword) ||
                    m.FLastName.Contains(vm.txtKeyword) ||
                    m.FEmail.Contains(vm.txtKeyword) ||
                    m.FMemberId.Contains(vm.txtKeyword))
                    .Include(m => m.FAccountStatusNavigation)
                    .Include(m => m.FArea)
                    .Include(m => m.FGender)
                    .OrderByDescending(m => m.FId);

                ViewBag.SearchKeyword = vm.txtKeyword;

                return View(await members.ToListAsync());
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
                .Include(m => m.FAccountStatusNavigation)
                .Include(m => m.FArea)
                .Include(m => m.FGender)
                .FirstOrDefaultAsync(m => m.FId == id);
            if (tMembers == null)
            {
                return NotFound();
            }

            return View(tMembers);
        }

        // GET: AdminMember/Create
        public IActionResult Create()
        {
            // todo 居住區域選單，要改成縣市跟區域，先選縣市再顯示區域
            ViewData["GenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType");
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName"); // 縣市選單的選項
            //ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName");
            ViewData["AccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus");
            return View();
        }

        // POST: AdminMember/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,LastName,FirstName,Email,Password,ConfirmPassword,DateOfBirth,GenderId,Cellphone,Address,City,Area,Introduction,AccountStatus,TotalMemberPoint,MemberPhoto")] CMemberCreateVM memberVM)
        {
            MemberService memberService = new MemberService(_context, _key);

            // 驗證Email是否已存在
            if (await memberService.IsEmailExistAsync(memberVM.Email))
            {
                ModelState.AddModelError("Email", "此Email已用過，請換一組");
            }

            if (ModelState.IsValid)
            {
                TMembers newMember = new TMembers();
                
                newMember.FMemberId = memberService.GenerateMemberId(); // 產生會員ID
                newMember.FSignUpTime = memberService.GenerateSignUpTime(); // 產生會員註冊時間

                // 產生會員點數
                if (memberVM.TotalMemberPoint == null)
                {
                    newMember.FTotalMemberPoint = 0;
                }

                newMember.FLastName = memberVM.LastName;
                newMember.FFirstName = memberVM.FirstName;
                newMember.FEmail = memberVM.Email;
                newMember.FDateOfBirth = memberVM.DateOfBirth;
                newMember.FGenderId = memberVM.GenderId;
                newMember.FCellphone = memberVM.Cellphone;
                newMember.FAreaId = memberVM.Area;
                newMember.FAddress = memberVM.Address;
                newMember.FIntroduction = memberVM.Introduction;
                newMember.FAccountStatus = memberVM.AccountStatus;
                newMember.FIsEmailVerified = true;

                // 加密會員密碼
                #region
                string password = memberVM.Password; // 要加密的密碼

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

            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", memberVM.GenderId);
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName", memberVM.City); // 縣市選單的選項
            ViewData["Areas"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", memberVM.Area);
            ViewData["AccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", memberVM.AccountStatus);
            return View(memberVM);
        }

        // GET: AdminMember/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var member = await _context.TMembers
                .Include(m => m.FGender)
                .Include(m => m.FArea.FCity)
                .FirstOrDefaultAsync(m => m.FId == id);

            if (member == null)
            {
                return NotFound();
            }

            CAdminMemberEditVM memberVM = new CAdminMemberEditVM
            {
                Id = member.FId,
                MemberId = member.FMemberId,
                LastName = member.FLastName,
                FirstName = member.FFirstName,
                Email = member.FEmail,
                DateOfBirth = member.FDateOfBirth,
                GenderId = member.FGenderId,
                Cellphone = member.FCellphone,
                City = member.FArea.FCityId,
                Area = member.FAreaId,
                Address = member.FAddress,
                MemberPhotoPath = member.FPhotoPath,
                Introduction = member.FIntroduction,
                AccountStatus = member.FAccountStatus,
                TotalMemberPoint = member.FTotalMemberPoint,
                SignUpTime = member.FSignUpTime
            };

            ViewBag.GenderId = new SelectList(_context.TGenders, "FGenderId", "FGenderType", member.FGenderId);

            // todo 縣市選單的選項不會預先選中指定值
            //ViewBag.Cities = new SelectList(_context.TCities, "FCityId", "FCityName", member.FArea.FCityId); // 縣市選單的選項
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName", memberVM.City); // 縣市選單的選項

            //ViewBag.Areas = new SelectList(_context.TAreas, "FId", "FAreaName", member.FAreaId);
            ViewData["Areas"] = new SelectList(await _context.TAreas.Where(a => a.FCityId == memberVM.City).ToListAsync(), "FId", "FAreaName", memberVM.Area); // 區域選單的選項

            ViewBag.AccountStatus = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", memberVM.AccountStatus);

            return View(memberVM);
        }

        // POST: AdminMember/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberId,LastName,FirstName,Email,DateOfBirth,GenderId,Cellphone,Address,City,Area,Introduction,AccountStatus,MemberPhoto")] CAdminMemberEditVM memberVM)
        {
            if (id != memberVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TMembers? member = _context.TMembers.FirstOrDefault(m => m.FId == memberVM.Id);

                    if (member != null)
                    {

                        member.FMemberId = memberVM.MemberId;
                        member.FLastName = memberVM.LastName;
                        member.FFirstName = memberVM.FirstName;
                        member.FEmail = memberVM.Email;
                        member.FDateOfBirth = memberVM.DateOfBirth;
                        member.FGenderId = memberVM.GenderId;
                        member.FCellphone = memberVM.Cellphone;
                        member.FAreaId = memberVM.Area;
                        member.FAddress = memberVM.Address;
                        member.FIntroduction = memberVM.Introduction;
                        member.FAccountStatus = memberVM.AccountStatus;

                        _context.Update(member);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TMembersExists(memberVM.Id))
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

            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", memberVM.GenderId);
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName"); // 縣市選單的選項
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", memberVM.Area);
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", memberVM.AccountStatus);

            return View(memberVM);
        }

        public async Task<IActionResult> Deactivate(int? id)
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

        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            if (_context.TMembers == null)
            {
                return Problem("Entity set 'InseparableContext.TMembers'  is null.");
            }
            var tMembers = await _context.TMembers.FindAsync(id);
            if (tMembers != null)
            {
                tMembers.FAccountStatus = 3; // 將會員狀態改為註銷狀態
                _context.TMembers.Update(tMembers);
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
