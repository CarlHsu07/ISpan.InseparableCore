﻿using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
    public class CMemberCenterVM
    {
        private TMembers _member;

        public TMembers member
        {
            get { return _member; }
            set { _member = value; }
        }

        public CMemberCenterVM()
        {
            _member = new TMembers();
        }

        //[Display(Name = "會員流水號")]
        public int Id { get => _member.FId; set => _member.FId = value; }

        [Display(Name = "會員編號")]
        public string? MemberId { get => _member.FMemberId; set => _member.FMemberId = value; }

        [Display(Name = "姓氏")]
        [Required(ErrorMessage = "請輸入姓氏")]
        public string LastName { get => _member.FLastName; set => _member.FLastName = value; }

        [Display(Name = "名字")]
        [Required(ErrorMessage = "請輸入名字")]
        public string FirstName { get => _member.FFirstName; set => _member.FFirstName = value; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "請輸入Email")]
        public string Email { get => _member.FEmail; set => _member.FEmail = value; }

        [Display(Name = "目前密碼")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "密碼長度至少為 {2} 個字元。", MinimumLength = 8)]
        [Required(ErrorMessage = "請輸入目前密碼")]
        public string? CurrentPassword { get => null; set => _member.FPasswordHash = value; }

        [Display(Name = "新密碼")]
        [Required(ErrorMessage = "請輸入新密碼")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+-=,./?;:'""[\]{}\\|]{8,}$", ErrorMessage = "密碼至少包含一個英文、一個數字，長度至少為8個字元。")]
        public string? NewPassword { get => null; set => _member.FPasswordHash = value; }

        [Display(Name = "確認新密碼")]
        [Required(ErrorMessage = "請再次輸入新密碼")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "密碼不相同！")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "生日")]
        public DateTime? DateOfBirth { get => _member.FDateOfBirth; set => _member.FDateOfBirth = value; }

        [Display(Name = "性別")]
        public string? GenderString { get; set; }

        [Display(Name = "性別")]
        public int? GenderId { get => _member.FGenderId; set => _member.FGenderId = value; }

        [Display(Name = "手機")]
        public string? Cellphone { get => _member.FCellphone; set => _member.FCellphone = value; }

        //[Display(Name = "縣市")]
        public string? CityString { get; set; }

        //[Display(Name = "縣市")]
        public int City { get; set; }

        //[Display(Name = "區域")]
        public string? AreaString { get; set; }

        //[Display(Name = "區域")]
        public int Area { get => _member.FAreaId; set => _member.FAreaId = value; }

        [Display(Name = "住址")]
        public string? Address { get => _member.FAddress; set => _member.FAddress = value; }

        [Display(Name = "會員大頭貼")]
        public string? PhotoPath { get => _member.FPhotoPath; set => _member.FPhotoPath = value; }

        [Display(Name = "上傳新大頭貼照")]
        public IFormFile? MemberPhoto { get; set; }

        [Display(Name = "自我介紹")]
        public string? Introduction { get => _member.FIntroduction; set => _member.FIntroduction = value; }

        [Display(Name = "註冊時間")]
        public DateTime SignUpTime { get => _member.FSignUpTime; set => _member.FSignUpTime = value; }

        [Display(Name = "會員狀態")]
        public string? AccountStatus { get; set; }

        [Display(Name = "點數")]
        public int? TotalMemberPoint { get => _member.FTotalMemberPoint; set => _member.FTotalMemberPoint = value; }


    }
}
