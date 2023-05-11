using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels.MemberVM
{
    /// <summary>
    /// 會員註冊的ViewModel
    /// </summary>
    public class CMemberRegisterVM
    {
        private TMembers _member;

        public TMembers member { get => _member; set => _member = value; }

        public CMemberRegisterVM() { _member = new TMembers(); }

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
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+-=,./?;:'""[\]{}\\|]{8,}$", ErrorMessage = "密碼至少包含一個英文、一個數字，長度至少為8個字元。")]
        //[StringLength(100, ErrorMessage = "密碼長度至少為 {2} 個字元。", MinimumLength = 8)]
        public string Password { get => _member.FPasswordHash; set => _member.FPasswordHash = value; }

        [Display(Name = "確認密碼")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "請再次輸入密碼")]
        [Compare("Password", ErrorMessage = "密碼不相同！")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "生日")]
        [DataType(DataType.Date)]
        [MaxToday(ErrorMessage = "生日不能超過今天")]
        public DateTime? DateOfBirth { get => _member.FDateOfBirth; set => _member.FDateOfBirth = value; }

        [Display(Name = "性別")]
        public int? GenderId { get => _member.FGenderId; set => _member.FGenderId = value; }

        //[Display(Name = "縣市")]
        [Required(ErrorMessage = "請選擇縣市")]
        public int City { get; set; }

        //[Display(Name = "地區")]
        [Required(ErrorMessage = "請選擇區域")]
        public int Area { get => _member.FAreaId; set => _member.FAreaId = value; }

        [Display(Name = "住址")]
        public string? Address { get => _member.FAddress; set => _member.FAddress = value; }

        public class MaxTodayAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                var date = (DateTime?)value;

                if (date.HasValue && date.Value.Date > DateTime.Today) { return false; }

                return true;
            }
        }

    }
}
