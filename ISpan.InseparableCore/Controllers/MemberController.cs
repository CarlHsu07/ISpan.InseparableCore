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
using System.Diagnostics.Metrics;
using prjMvcCoreDemo.Models;
using System.Text.Json;
using System.Security.Claims;

namespace ISpan.InseparableCore.Controllers
{
    public class MemberController : SuperController
    {
        private readonly InseparableContext _context;
        IWebHostEnvironment _enviro;

        public MemberController(InseparableContext context, IWebHostEnvironment enviro)
        {
            _context = context;
            _enviro = enviro;
        }

        // GET: Member/Index/2
        public async Task<IActionResult> Index(int? id)
        {
            string genderString = string.Empty;
            string cityString = string.Empty;
            string areaString = string.Empty;
            string accountStatusString = string.Empty;

            if (id == null || _context.TMembers == null)
            {
                return RedirectToAction(nameof(HomeController.Login), "Home", null);
            }

            var member = await _context.TMembers.FindAsync(id);

            if (member == null) // 沒找到會員
            {
                return NotFound();
            }

            if (member.FGenderId != null) // 取性別
            {
                genderString = _context.TGenders
                    .Where(g => g.FGenderId == member.FGenderId)
                    .Select(x => x.FGenderType)
                    .FirstOrDefault()
                    .ToString();
            }

            if (member.FAreaId != null) // 取縣市
            {
                cityString = _context.TAreas
                    .Where(a => a.FId == member.FAreaId)
                    .Select(x => x.FCity.FCityName)
                    .FirstOrDefault()
                    .ToString();
            }

            if (member.FAreaId != null) // 取地區
            {
                areaString = _context.TAreas
                    .Where(a => a.FId == member.FAreaId)
                    .Select(x => x.FAreaName)
                    .FirstOrDefault()
                    .ToString();
            }

            if (member.FAccountStatus != null) // 取會員狀態
            {
                accountStatusString = _context.TAccountStatuses
                    .Where(s => s.FStatusId == member.FAccountStatus)
                    .Select(x => x.FStatus)
                    .FirstOrDefault()
                    .ToString();
            }

            // 將資料庫中的 TMembers 物件映射到 ViewModel（即CEditProfileViewModel）
            var viewModel = new CMemberCenterViewModel
            {
                // 設定 ViewModel 的屬性值
                Id = member.FId,
                MemberId = member.FMemberId,
                LastName = member.FLastName,
                FirstName = member.FFirstName,
                Email = member.FEmail,
                DateOfBirth = member.FDateOfBirth,
                GenderString = genderString,
                GenderId = member.FGenderId,
                Cellphone = member.FCellphone,
                CityString = cityString,
                AreaString = areaString,
                Address = member.FAddress,
                PhotoPath = member.FPhotoPath,
                Introduction = member.FIntroduction,
                AccountStatus = accountStatusString,
                TotalMemberPoint = member.FTotalMemberPoint,
                SignUpTime = member.FSignUpTime

            };

            int? CityID = null;
            if (member.FAreaId != null)
            {
                CityID = _context.TAreas.Where(a => a.FId == member.FAreaId).Select(x => x.FCityId).FirstOrDefault();
            }

            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", member.FGenderId);
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName", CityID); // 縣市選單的選項
            ViewData["Areas"] = new SelectList(_context.TAreas, "FId", "FAreaName", member.FAreaId); // 區域選單的選項
            
            return View(viewModel);
        }

        // GET: Member/OrderHistorys/2
        public async Task<IActionResult> OrderHistorys(int? id)
        {
            if (id == null || _context.TOrders == null)
            {
                return NotFound();
            }

            var orders = await _context.TOrders
                .Include(t => t.TProductOrderDetails)
                .Include(t => t.TTicketOrderDetails)
                .Where(m => m.FMemberId == id)
                .ToListAsync();

            if (orders == null)
            {
                return NotFound();
            }

            var viewModel = orders.Select(o => new CMemberOrderHistoryViewModel
            {
                OrderId = o.FOrderId,
                CinemaId = o.FCinemaId,
                OrderDate = o.FOrderDate,
                ModifiedTime = o.FModifiedTime,
                TotalMoney = o.FTotalMoney,
                Status = o.FStatus
            }).ToList();

            return View(viewModel);
        }

        // GET: Member/Profile/5
        public async Task<IActionResult> Profile(int? id)
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

        // POST: Member/AddFriend/7
        [HttpPost]
        public IActionResult AddFriend(int friendId)
        {
            int? memberId = GetMemberID();
            if (memberId != null)
            {
                TMembers friend = _context.TMembers.FirstOrDefault(m => m.FId == friendId);
                if (friend == null)
                {
                    return Json(new { success = false, message = "加好友失敗 in C#" });
                }

                // 做加好友的操作，向DB中的 tFriends 中插入一條記錄
                var friendship = new TFriends
                {
                    FMemberId = (int)memberId,
                    FFriendId = friend.FId,
                    FFriendDateTime = DateTime.Now
                };
                _context.TFriends.Add(friendship);
                _context.SaveChanges();

                // 返回 JSON 格式的結果
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "加好友失敗 in C#" });
            }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(/*[Bind("FLastName,FFirstName,FEmail,FPasswordHash,FDateOfBirth,FGenderId,FCellphone,FAddress,FAreaZipCode")]*/ CMemberRegisterViewModel MemberIn)
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
            if (member == null) // 沒找到會員
            {
                return NotFound();
            }


            // 將資料庫中的 TMembers 物件映射到 ViewModel（即CEditProfileViewModel）
            var viewModel = new CMemberEditProfileViewModel
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
                PhotoPath = member.FPhotoPath,
                Introduction = member.FIntroduction

            };

            int? cityID = null;
            if (member.FAreaId != null)
            {
                cityID = _context.TAreas.Where(a => a.FId == member.FAreaId).Select(x => x.FCityId).FirstOrDefault();
            }

            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", member.FGenderId);
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName", cityID); // 產生縣市選單的選項
            ViewData["Areas"] = new SelectList(_context.TAreas, "FId", "FAreaName", member.FAreaId); // 產生區域選單的選項

            return View(viewModel);
        }

        // POST: Member/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, [Bind("Id,MemberId,LastName,FirstName,Email,Password,DateOfBirth,GenderId,Cellphone,Address,Area,PhotoPath,Introduction,MemberPhoto")] CMemberEditProfileViewModel MemberIn)
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
                        if (MemberIn.MemberPhoto != null) // todo 有些圖片不會正確存？
                        {
                            string extension = Path.GetExtension(MemberIn.MemberPhoto.FileName).ToLower();
                            string photoName = "memberProfilePhotos_" + member.FMemberId.ToString() + extension;
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
                        // todo 字數太長的處理

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromBody] CChangePasswordViewModel MemberIn)
        {
            TMembers member = _context.TMembers.FirstOrDefault(m => m.FId == MemberIn.Id);

            if (member == null)
            {
                return NotFound();
            }

            if (!String.IsNullOrEmpty(MemberIn.CurrentPassword))
            {
                // 驗證目前密碼是否正確
                if (!CPasswordHelper.VerifyPassword(MemberIn.CurrentPassword, member.FPasswordHash, member.FPasswordSalt))
                {
                    ModelState.AddModelError("CurrentPassword", "目前密碼有誤");
                    return Json(new { success = false, message = 1 }); // "目前密碼有誤 in C#"
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!String.IsNullOrEmpty(MemberIn.NewPassword))
                    {
                        string password = MemberIn.NewPassword; // 要加密的密碼

                        byte[] salt = CPasswordHelper.GenerateSalt(); // 產生鹽值

                        byte[] hashedPassword = CPasswordHelper.HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), salt); // 密碼與鹽結合後加密

                        // 將鹽值與加密後的密碼轉換成 Base64 字串儲存
                        member.FPasswordSalt = Convert.ToBase64String(salt);
                        member.FPasswordHash = Convert.ToBase64String(hashedPassword);
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

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "更改密碼失敗 in C#" });
            }
        }

        // GET: Home/Logout
        public IActionResult Logout()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                HttpContext.Session.Remove(CDictionary.SK_LOGINED_USER);
                return RedirectToAction(nameof(HomeController.Login), "Home");
            }

            return View();
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

        // 從Session中取得會員的fID
        private int? GetMemberID()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                var serializedTMembers = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                var member = JsonSerializer.Deserialize<TMembers>(serializedTMembers);
                return member.FId;
            }

            return null;
        }
    }
}
