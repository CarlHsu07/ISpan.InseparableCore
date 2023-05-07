using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
    public class CAdminMemberEditVM
    {
        private TMembers _member;

        public TMembers members { get => _member; set => _member = value; }

        public CAdminMemberEditVM() { _member = new TMembers(); }

        public int Id { get => _member.FId; set => _member.FId = value; }

        /// <summary>
        /// 會員ID
        /// </summary>
        [Display(Name = "會員ID")]
        public string MemberId { get => _member.FMemberId; set => _member.FMemberId = value; }

        /// <summary>
        /// 姓氏
        /// </summary>
        [Display(Name = "姓氏")]
        public string LastName { get => _member.FLastName; set => _member.FLastName = value; }

        /// <summary>
        /// 名字
        /// </summary>
        [Display(Name = "名字")]
        public string FirstName { get => _member.FFirstName; set => _member.FFirstName = value; }

        /// <summary>
        /// 電子郵件的地址
        /// </summary>
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "請輸入正確的Email格式")]
        [Required(ErrorMessage = "請輸入Email")]
        public string Email { get => _member.FEmail; set => _member.FEmail = value; }

        /// <summary>
        /// 密碼加密值
        /// </summary>
        //[Display(Name = "密碼")]
        //[Required(ErrorMessage = "請輸入密碼")]
        //[DataType(DataType.Password)]
        //public string Password { get => _member.FPasswordHash; set => _member.FPasswordHash = value; }

        //[Display(Name = "確認密碼")]
        //[Required(ErrorMessage = "請再次輸入密碼")]
        //[Compare("Password", ErrorMessage = "密碼不相同！")]
        //[DataType(DataType.Password)]
        //public string? ConfirmPassword { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Display(Name = "生日")]
        public DateTime? DateOfBirth { get => _member.FDateOfBirth; set => _member.FDateOfBirth = value; }

        /// <summary>
        /// 性別ID
        /// </summary>
		[Display(Name = "性別")]
        public int? GenderId { get => _member.FGenderId; set => _member.FGenderId = value; }

        /// <summary>
        /// 手機號碼
        /// </summary>
        [Display(Name = "手機")]
        public string? Cellphone { get => _member.FCellphone; set => _member.FCellphone = value; }

        /// <summary>
        /// 住址
        /// </summary>
        [Display(Name = "住址")]
        public string? Address { get => _member.FAddress; set => _member.FAddress = value; }

        [Required(ErrorMessage = "請選擇縣市")]
        public int? CityId { get; set; }

        /// <summary>
        /// 區域ID
        /// </summary>
        [Required(ErrorMessage = "請選擇區域")]
        public int? AreaId { get => _member.FAreaId; set => _member.FAreaId = value; }

        /// <summary>
        /// 大頭貼的檔案路徑
        /// </summary>
        public string? MemberPhotoPath { get => _member.FPhotoPath; set => _member.FPhotoPath = value; }

        /// <summary>
        /// 個人簡介
        /// </summary>
        [Display(Name = "個人簡介")]
        [MaxLength(500, ErrorMessage = "個人簡介不能超過{0}字")]
        public string? Introduction { get => _member.FIntroduction; set => _member.FIntroduction = value; }

        /// <summary>
        /// 會員帳戶狀態
        /// </summary>
        [Display(Name = "會員狀態")]
        [Required(ErrorMessage = "請選擇會員狀態")]
        public int? AccountStatus { get => _member.FAccountStatus; set => _member.FAccountStatus = value; }

        /// <summary>
        /// 目前點數餘額
        /// </summary>
        public int? TotalMemberPoint { get => _member.FTotalMemberPoint; set => _member.FTotalMemberPoint = value; }

        /// <summary>
        /// 會員註冊時間
        /// </summary>
        public DateTime? SignUpTime { get => _member.FSignUpTime; set => _member.FSignUpTime = value; }

        [Display(Name = "上傳新大頭貼照")]
        public IFormFile? MemberPhoto { get; set; }
    }
}
