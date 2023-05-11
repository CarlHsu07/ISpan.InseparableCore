using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;
using static ISpan.InseparableCore.ViewModels.CMemberRegisterVM;

namespace ISpan.InseparableCore.ViewModels
{
    public class CMemberCreateVM
    {
        private TMembers _member;

        public TMembers member { get => _member; set => _member = value; }

        public CMemberCreateVM() { _member = new TMembers(); }

        [Display(Name = "會員ID")]
        public string? MemberId { get => _member.FMemberId; set => _member.FMemberId = value; }

        [Display(Name = "姓氏")]
        [Required(ErrorMessage = "請輸入姓氏")]
        public string LastName { get => _member.FLastName; set => _member.FLastName = value; }

        [Display(Name = "名字")]
        [Required(ErrorMessage = "請輸入名字")]
        public string FirstName { get => _member.FFirstName; set => _member.FFirstName = value; }

        [Display(Name = "Email(帳號)")]
        [EmailAddress(ErrorMessage = "請輸入正確的Email格式")]
        [Required(ErrorMessage = "請輸入Email")]
        public string Email { get => _member.FEmail; set => _member.FEmail = value; }

        [Display(Name = "密碼(密碼需包含英文、數字，至少為8位數)")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "請輸入密碼")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+-=,./?;:'""[\]{}\\|]{8,}$", ErrorMessage = "格式錯誤，密碼至少包含一個英文、一個數字，長度至少為8個字元。")]
        public string Password { get => _member.FPasswordHash; set => _member.FPasswordHash = value; }

        [Display(Name = "確認密碼")]
        [Required(ErrorMessage = "請再次輸入密碼")]
        [Compare("Password", ErrorMessage = "密碼不相同！")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "生日")]
        [DataType(DataType.Date)]
        [MaxToday(ErrorMessage = "生日不能超過今天")]
        public DateTime? DateOfBirth { get => _member.FDateOfBirth; set => _member.FDateOfBirth = value; }

        [Display(Name = "性別")]
        public int? GenderId { get => _member.FGenderId; set => _member.FGenderId = value; }

        [Display(Name = "手機")]
        public string? Cellphone { get => _member.FCellphone; set => _member.FCellphone = value; }

        [Display(Name = "縣市")]
        [Required(ErrorMessage = "請選擇縣市")]
        public int City { get; set; }

        [Display(Name = "地區")]
        [Required(ErrorMessage = "請選擇區域")]
        public int Area { get => _member.FAreaId; set => _member.FAreaId = value; }

        [Display(Name = "住址")]
        public string? Address { get => _member.FAddress; set => _member.FAddress = value; }

        //public string? PhotoPath
        //{
        //    get { return _member.FPhotoPath; }
        //    set { _member.FPhotoPath = value; }
        //}

        [Display(Name = "自我介紹")]
        public string? Introduction { get => _member.FIntroduction; set => _member.FIntroduction = value; }

        [Display(Name = "會員狀態")]
        [Required(ErrorMessage = "請選擇會員狀態")]
        public int AccountStatus { get => _member.FAccountStatus; set => _member.FAccountStatus = value; }

        [Display(Name = "會員點數")]
        public int? TotalMemberPoint { get => _member.FTotalMemberPoint; set => _member.FTotalMemberPoint = value; }

        public DateTime SignUpTime { get => _member.FSignUpTime; set => _member.FSignUpTime = value; }

        [Display(Name = "上傳新大頭貼照")]
        public IFormFile? MemberPhoto { get; set; }
    }
}
