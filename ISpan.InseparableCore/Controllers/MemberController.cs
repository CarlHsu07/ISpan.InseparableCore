using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models;
using System.Text;
using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using System.Text.Json;
using ISpan.InseparableCore.Models.DAL.Repo;
using NuGet.Protocol;
using System.Text.Json.Serialization;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;

namespace ISpan.InseparableCore.Controllers
{
    public class MemberController : SuperController
    {
        private readonly InseparableContext _context;
        private readonly ApiKeys _key;
        private readonly OrderRepository _orderRepo;
        private readonly TicketOrderRepository _ticketRepo;
        private readonly ProductOrderRepository _productRepo;
        IWebHostEnvironment _enviro;

        public MemberController(InseparableContext context, IWebHostEnvironment enviro, IOptions<ApiKeys> key)
        {
            _context = context;
            _orderRepo = new OrderRepository(context);
            _ticketRepo = new TicketOrderRepository(context);
            _productRepo = new ProductOrderRepository(context);
            _enviro = enviro;
            _key = key.Value;
        }

        // GET: Member/Index/2
        public async Task<IActionResult> Index()
        {
            string genderString = string.Empty;
            string cityString = string.Empty;
            string areaString = string.Empty;
            string accountStatusString = string.Empty;
            int? memberId = GetMemberFID();

            if (memberId == null || _context.TMembers == null)
            {
                return RedirectToAction(nameof(HomeController.Login), "Home", null);
            }

            var member = await _context.TMembers
                .Include(m => m.FGender)
                .Include(m => m.FArea.FCity)
                .Include(m => m.FArea)
                //.Include(m => m.FAccountStatus)
                .Where(m => m.FId == memberId)
                .FirstOrDefaultAsync();

            if (member == null) // 沒找到會員
            {
                return RedirectToAction(nameof(HomeController.Login), "Home", null);
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
            var memberVM = new CMemberCenterVM
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
                City = member.FArea.FCityId,
                AreaString = areaString,
                Area = member.FAreaId,
                Address = member.FAddress,
                PhotoPath = member.FPhotoPath,
                Introduction = member.FIntroduction,
                AccountStatus = accountStatusString,
                TotalMemberPoint = member.FTotalMemberPoint,
                SignUpTime = member.FSignUpTime
            };

            //int FCityId = 0;
            //if (member.FAreaId != null)
            //{
            //    FCityId = await _context.TAreas.Where(a => a.FId == member.FAreaId).Select(x => x.FCityId).FirstOrDefaultAsync();
            //}

            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", memberVM.GenderId);
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName", memberVM.City); // 縣市選單的選項
            ViewData["Areas"] = new SelectList(await _context.TAreas.Where(a => a.FCityId == memberVM.City).ToListAsync(), "FId", "FAreaName", member.FAreaId); // 區域選單的選項

            return View(memberVM);
        }

        //todo 以下是member會員中心的訂單紀錄 in Member(已複製好)
        public IPagedList<COrderVM> MemberOrderPageList(int? pageIndex, int? pageSize, List<COrderVM> vm)
        {
            if (!pageIndex.HasValue || pageIndex < 1)
                return null;
            IPagedList<COrderVM> pagelist = vm.ToPagedList(pageIndex ?? 1, (int)pageSize);
            if (pagelist.PageNumber != 1 && pageIndex.HasValue && pageIndex > pagelist.PageCount)
                return null;
            return pagelist;
        }
        public IActionResult OrderHistory()
        {
            TMembers member = new TMembers();
            string json = string.Empty;
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                member = JsonSerializer.Deserialize<TMembers>(json);
            }
            if (member == null)
                return RedirectToAction(nameof(HomeController.Login), "Home", null);

            var data = _orderRepo.GetMemberOrder(member.FId, null);
            var pagesize = 5;
            var pageIndex = 1;

            var pagedItems = data.Skip((pageIndex - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.page = MemberOrderPageList(pageIndex, pagesize, data);

            return View(pagedItems);
        }
        [HttpPost]
        public IActionResult OrderHistory(MemberOrderSearch search)
        {
            TMembers member = new TMembers();
            string json = string.Empty;
            // todo 改用super給的_user
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                member = JsonSerializer.Deserialize<TMembers>(json);
            }
            if (member == null)
                return RedirectToAction(nameof(HomeController.Login), "Home", null);

            var data = _orderRepo.GetMemberOrder(member.FId, search);
            var pagesize = 5;
            var pageIndex = search.pageindex;

            var pagedItems = data.Skip((pageIndex - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.page = MemberOrderPageList(pageIndex, pagesize, data);

            var count = data.Count();
            var totalpage = (int)Math.Ceiling(count / (double)pagesize);  //無條件進位
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string jsons = JsonSerializer.Serialize(pagedItems, options);
            return Ok(new
            {
                Items = jsons,
                totalpage = totalpage,
            }.ToJson());
        }
        public IActionResult MemberOrderDetail(int? id)
        {
            CorderDetaillVM vm = new CorderDetaillVM();
            if (id == null)
                return View();

            vm.orders = _orderRepo.GetOneOrder(id);
            vm.ticket = _ticketRepo.GetById(id);
            vm.product = _productRepo.GetById(id);


            if (id == null || _context.TOrders == null)
            {
                return RedirectToAction(nameof(OrderHistory));
            }

            return View(vm);
        }
        public IActionResult MemberOrderDelete(int? id)
        {
            if (id == null || _context.TOrders == null)
            {
                return RedirectToAction(nameof(OrderHistory));
            }
            COrderVM vm = new COrderVM();
            vm.orders = _orderRepo.GetOneOrder(id);
            vm.FCinema = vm.orders.FCinema;
            vm.FMember = vm.orders.FMember;
            if (vm.orders == null)
            {
                return RedirectToAction(nameof(OrderHistory));
            }

            return View(vm);
        }

        [HttpPost, ActionName("MemberOrderDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MemberOrderDeleteConfirmed(int id)
        {
            if (_context.TOrders == null)
            {
                return Problem("Entity set 'InseparableContext.TOrders'  is null.");
            }
            try
            {
                _orderRepo.Delete(id);

            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return RedirectToAction("MemberOrderDelete", new { id });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OrderHistory));
        }

        // 會員檔案
        // GET: Member/Profile/5
        public async Task<IActionResult> Profile(int? id)
        {
            MemberService memberService = new MemberService(_context, _key);
            int? memberId = GetMemberFID();

            if (id == null || _context.TMembers == null)
            {
                return RedirectToAction(nameof(MemberController.Index), "Member");
            }

            var member = await _context.TMembers
                .Include(t => t.FAccountStatusNavigation)
                .Include(t => t.FArea)
                .Include(t => t.FGender)
                .FirstOrDefaultAsync(m => m.FId == id);

            if (member == null)
            {
                return RedirectToAction(nameof(MemberController.Index), "Member");
            }

            bool isFriend = memberService.IsFriend(memberId, member.FId);
            bool isSameMember = memberService.IsSameMember(memberId, member.FId);
            bool isLogedIn = memberId != null;

            ViewData["FriendStatus"] = isFriend ? "is-friend" : "not-friend";
            ViewData["FriendBtnText"] = isFriend ? "已是好友" : "加入好友";
            ViewData["isSameMember"] = isSameMember;
            ViewData["isLogedIn"] = isLogedIn;

            return View(member);
        }

        // AJAX 加入好友
        [HttpPost]
        public IActionResult AddFriend(int friendId)
        {
            int? memberId = GetMemberFID();
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

        // AJAX 解除好友
        [HttpPost]
        public IActionResult UnFriend(int friendId)
        {
            MemberService memberService = new MemberService(_context, _key);
            int? memberId = GetMemberFID();

            if (memberId != null && memberService.IsFriend(memberId, friendId))
            {
                var friendShip = memberService.GetOneFriendShip((int)memberId, friendId);

                if (friendShip == null)
                {
                    return Json(new { success = false, message = "找不到好友關係 in C#" });
                }

                _context.TFriends.Remove(friendShip); // 刪除好友關係資料
                _context.SaveChanges();

                return Json(new { success = true }); // 返回 JSON 格式的結果
            }
            else
            {
                return Json(new { success = false, message = "取消好友失敗 in C#" });
            }
        }

        // todo 在View中把大頭貼照改成正圓
        // GET: Member/FriendList
        public async Task<IActionResult> FriendList()
		{
            MemberService memberService = new MemberService(_context, _key);
            int? memberId = GetMemberFID();

            if (memberId != null)
            {
                ViewBag.memberId = memberId;
                List <CFriendListViewModel> friendList = await memberService.GetFriendListAsync(memberId);
                return View(friendList);
            }

            return RedirectToAction(nameof(HomeController.Login), "Home");
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
        public async Task<IActionResult> Register([Bind("LastName,FirstName,Email,Password,ConfirmPassword,DateOfBirth,GenderId,City,Area,Address")] CMemberRegisterVM memberVM)
        {
            TMembers newMember = new TMembers();
            MemberService memberService = new MemberService(_context, _key);

            // 驗證Email是否已存在
            if (await memberService.IsEmailExistAsync(memberVM.Email))
            {
                ModelState.AddModelError("Email", "此Email已用過，請換一組");
            }

            if (ModelState.IsValid)
            {
                newMember.FMemberId = memberService.GenerateMemberId(); // 產生會員ID
                newMember.FSignUpTime = memberService.GenerateSignUpTime(); // 產生會員註冊時間
                newMember.FVerificationCode = memberService.GenerateVerificationCode();
                newMember.FIsEmailVerified = false;
                newMember.FAccountStatus = 1;
                newMember.FTotalMemberPoint = 0; // 產生會員點數

                newMember.FLastName = memberVM.LastName;
                newMember.FFirstName = memberVM.FirstName;
                newMember.FEmail = memberVM.Email;
                newMember.FDateOfBirth = memberVM.DateOfBirth;
                newMember.FGenderId = memberVM.GenderId;
                newMember.FAreaId = memberVM.Area;
                newMember.FAddress = memberVM.Address;

                //加密會員密碼
                #region 
                string password = memberVM.Password; // 要加密的密碼
                
                byte[] salt = CPasswordHelper.GenerateSalt(); // 產生鹽值

                // 將密碼與鹽值結合後進行加密
                byte[] hashedPassword = CPasswordHelper.HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), salt);

                // 將鹽值與加密後的密碼轉換成 Base64 字串儲存
                newMember.FPasswordSalt = Convert.ToBase64String(salt);
                newMember.FPasswordHash = Convert.ToBase64String(hashedPassword);
                #endregion

                string url = memberService.GenerateEmailVerificationLink(newMember.FMemberId, newMember.FVerificationCode);
                memberService.SendVerificationEmail(memberVM.Email, url); // 寄驗證信

                _context.Add(newMember);
                await _context.SaveChangesAsync();

                return View("RegistrationCompleted");
            }

            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", newMember.FAccountStatus);
            ViewData["Cities"] = new SelectList(_context.TCities, "FCityId", "FCityName", memberVM.City); // 縣市選單的選項
            ViewData["Areas"] = new SelectList(await _context.TAreas.Where(a => a.FCityId == memberVM.City).ToListAsync(), "FId", "FAreaName", memberVM.Area);
            //ViewData["Areas"] = await _context.TAreas.Where(a => a.FCityId == memberVM.City)
            //    .Select(a => new SelectListItem
            //    {
            //        Value = a.FId.ToString(),
            //        Text = a.FAreaName,
            //        Selected = (a.FId == memberVM.Area)
            //    }).ToListAsync();
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", memberVM.GenderId);
            return View(memberVM);
        }

        //public async Task<IActionResult> RegisterC()
        //{
        //    return View("RegistrationCompleted");
        //}

        // GET: Member/EditProfile/5
        public async Task<IActionResult> EditProfile(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return RedirectToAction(nameof(HomeController.Login), "Home");
            }

            var member = await _context.TMembers.FindAsync(id);
            if (member == null) // 沒找到會員
            {
                return RedirectToAction(nameof(HomeController.Login), "Home");
            }

            // 將資料庫中的 TMembers 物件映射到 ViewModel（即CEditProfileViewModel）
            var viewModel = new CMemberEditProfileVM
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, [Bind("Id,MemberId,LastName,FirstName,Email,Password,DateOfBirth,GenderId,Cellphone,Address,City,Area,PhotoPath,Introduction,MemberPhoto")] CMemberEditProfileVM MemberIn)
        {
            if (id != MemberIn.Id)
            {
                return RedirectToAction(nameof(HomeController.Login), "Home");
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
                        // todo 字數太長的處理，目前已在VM驗證

                        if (MemberIn.Password != null) // 加密會員密碼
                        {
                            string password = MemberIn.Password; // 要加密的密碼

                            byte[] salt = CPasswordHelper.GenerateSalt(); // 產生鹽值

                            byte[] hashedPassword = CPasswordHelper.HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), salt); // 密碼與鹽結合後加密

                            // 將鹽值與加密後的密碼轉換成 Base64 字串儲存
                            member.FPasswordSalt = Convert.ToBase64String(salt);
                            member.FPasswordHash = Convert.ToBase64String(hashedPassword);
                        }

                        _context.Update(member);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TMembersExists(MemberIn.Id))
                    {
                        return RedirectToAction(nameof(HomeController.Login), "Home");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["Areas"] = new SelectList(_context.TAreas, "FId", "FAreaName", MemberIn.Area);
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", MemberIn.GenderId);
            return View();
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

        /// <summary>
        /// 驗證會員信箱
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string memberId, string token)
        {
            MemberService memberService = new MemberService(_context, _key);

            if ( String.IsNullOrEmpty(memberId) || String.IsNullOrEmpty(token)) // memberId或token是null或空字串時就顯示錯誤畫面
            {

                ViewBag.IsConfirmEmailSuccess = false;
                return View("VerifyEmail");

                //return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var member = await _context.TMembers.FirstOrDefaultAsync(m => m.FMemberId == memberId);
            if (member == null)
            {
                return RedirectToAction(nameof(HomeController.Login), "Home");
            }

            if (await memberService.ConfirmEmail(member, token))
            {
                ViewBag.IsConfirmEmailSuccess = true;
                return View("VerifyEmail");
            }
            else
            {
                ViewBag.IsConfirmEmailSuccess = false;
                return View("VerifyEmail");

                //throw new ApplicationException($"驗證電子郵件時發生錯誤 for user with ID '{memberId}':");
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

        // AJAX 取得指定縣市的區域(鄉鎮市區)
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
        private int? GetMemberFID()
        {
            // todo 改用super給的_user

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
