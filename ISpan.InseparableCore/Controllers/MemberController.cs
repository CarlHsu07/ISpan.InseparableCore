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
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace ISpan.InseparableCore.Controllers
{
    public class MemberController : Controller
    {
        private readonly InseparableContext _context;
        IWebHostEnvironment _enviro;

        public MemberController(InseparableContext context, IWebHostEnvironment enviro)
        {
            _context = context;
            _enviro = enviro;
        }

        // GET: Member
        public async Task<IActionResult> Index()
        {
            var inseparableContext = _context.TMembers.Include(t => t.FAccountStatusNavigation).Include(t => t.FArea).Include(t => t.FGender);
            return View(await inseparableContext.ToListAsync());
        }

        // GET: Member/Details/5
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

		// GET: Member/ViewProfile/5
		public async Task<IActionResult> ViewProfile(int? id)
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

		// GET: Member/Register
		public IActionResult Register()
        {
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName"); // 縣市選單的選項
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

                newMember.FMemberId = memberService.GenerateMemberId(); // 產生會員ID
                newMember.FSignUpTime = memberService.GenerateSignUpTime(); // 產生會員註冊時間

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
                newMember.FAddress = MemberIn.Address;


                // 加密會員密碼
                #region
                string password = MemberIn.Password; // 要加密的密碼
                
                byte[] salt = CPasswordHelper.GenerateSalt(); // 產生鹽值

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

        // GET: Member/EditProfile/5
        public async Task<IActionResult> EditProfile(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var member = await _context.TMembers.FindAsync(id);
            if (member == null) // 用id沒找到會員
            {
                return NotFound();
            }


            // 將資料庫中的 TMembers 物件映射到 ViewModel（即CEditProfileViewModel）
            var viewModel = new CEditProfileViewModel
            {
                // 設定 ViewModel 的屬性值
                Id = member.FId,
                MemberId = member.FMemberId,
                LastName = member.FLastName,
                FirstName = member.FFirstName,
                Email = member.FEmail,
                Password = "",
                DateOfBirth = member.FDateOfBirth,
                GenderId = member.FGenderId,
                Cellphone = member.FCellphone,
                Area = member.FAreaId,
                Address = member.FAddress,
                Introduction = member.FIntroduction

            };
            int? cityID = null;
            if (member.FAreaId != null)
            {
                cityID = _context.TAreas.Where(a => a.FId == member.FAreaId).Select(x => x.FCityId).FirstOrDefault();
            }

            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName", cityID); // 縣市選單的選項
            ViewData["Areas"] = new SelectList(_context.TAreas, "FId", "FAreaName", member.FAreaId); // 區域選單的選項
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", member.FGenderId);

            //ViewData["Cities"] = ; // 
            return View(viewModel);
        }

        // POST: Member/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, [Bind("Id,MemberId,LastName,FirstName,Email,Password,DateOfBirth,GenderId,Cellphone,Address,Area,PhotoPath,Introduction,MemberPhoto")] CEditProfileViewModel MemberIn)
        {
            if (id != MemberIn.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TMembers member = _context.TMembers.FirstOrDefault(m => m.FId == MemberIn.Id);
                    if (member != null)
                    {
                        if (MemberIn.MemberPhoto != null) // todo 圖片不會正確存
                        {
                            string type = MemberIn.MemberPhoto.ContentType;
                            string photoName = "memberProfilePhotos_" + Guid.NewGuid().ToString() + ".jpg";
                            string photoPath = _enviro.WebRootPath + "/images/memberProfilePhotos/" + photoName;
                            MemberIn.MemberPhoto.CopyTo(new FileStream(photoPath, FileMode.Create));
                            member.FPhotoPath = photoName;
                        }

                        member.FLastName = MemberIn.LastName;
                        member.FFirstName = MemberIn.FirstName;
                        member.FEmail = MemberIn.Email;
                        member.FDateOfBirth = MemberIn.DateOfBirth;
                        member.FGenderId = MemberIn.GenderId;
                        member.FCellphone = MemberIn.Cellphone;
                        member.FAreaId = MemberIn.Area;
                        member.FAddress = MemberIn.Address;
                        member.FIntroduction = MemberIn.Introduction;

                        if (MemberIn.Password != null) // 加密會員密碼
                        {
                            string password = MemberIn.Password; // 要加密的密碼

                            byte[] salt = CPasswordHelper.GenerateSalt(); // 產生鹽值

                            byte[] hashedPassword = CPasswordHelper.HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), salt); // 密碼與鹽結合後加密

                            // 將鹽值與加密後的密碼轉換成 Base64 字串儲存
                            member.FPasswordSalt = Convert.ToBase64String(salt);
                            member.FPasswordHash = Convert.ToBase64String(hashedPassword);
                        }
                    }

                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TMembersExists(MemberIn.Id))
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
            
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", MemberIn.Area);
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", MemberIn.GenderId);
            return View(MemberIn);
        }


        // GET: Member/Delete/5
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

        // POST: Member/Delete/5
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
        public IActionResult GetAreas(int cityId)
        {
            // 根據縣市值，查詢對應的區域資料
            var areas = _context.TAreas
                .Where(a => a.FCityId == cityId)
                .Select(a => new
                {
                    areaID = a.FId,
                    //cityID = a.FCityId,
                    areaName = a.FAreaName
                })
                .ToList();

            // 將區域資料轉換成 JSON 格式回傳給前端
            return Json(areas);
        }
    }
}
