using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;
using static ISpan.InseparableCore.ViewModels.CMemberRegisterVM;

namespace ISpan.InseparableCore.ViewModels
{
	public class CMemberEditProfileVM
	{
		private TMembers _member;

		public TMembers member
		{
			get { return _member; } set { _member = value; }
		}

		public CMemberEditProfileVM()
		{
			_member = new TMembers();
		}

		//[Display(Name = "會員流水號")]
		public int Id
		{
			get { return _member.FId; }
			set { _member.FId = value; }
		}

		[Display(Name = "會員編號(不可修改)")]
		public string? MemberId
		{
			get { return _member.FMemberId; } set { _member.FMemberId = value; }
		}

		[Display(Name = "姓氏")]
		[Required(ErrorMessage = "請輸入姓氏")]
		public string LastName
		{
			get { return _member.FLastName; } set { _member.FLastName = value; }
		}

		[Display(Name = "名字")]
		[Required(ErrorMessage = "請輸入名字")]
		public string FirstName
		{
			get { return _member.FFirstName; } set { _member.FFirstName = value; }
		}

		[Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "請輸入正確的Email格式")]
        [Required(ErrorMessage = "請輸入Email")]
		public string Email
		{
			get { return _member.FEmail; } set { _member.FEmail = value; }
		}

		[Display(Name = "密碼")]
		[DataType(DataType.Password)]
		public string? Password
		{
			get { return null; } set { _member.FPasswordHash = value; }
		}

		[Display(Name = "確認密碼")]
		[Compare("Password", ErrorMessage = "密碼不相同！")]
		[DataType(DataType.Password)]
		public string? ConfirmPassword { get; set; }

		[Display(Name = "生日")]
        [MaxToday(ErrorMessage = "生日不能超過今天")]
        public DateTime? DateOfBirth
		{
			get { return _member.FDateOfBirth; } set { _member.FDateOfBirth = value; }
		}

		[Display(Name = "性別")]
		public int? GenderId
		{
			get { return _member.FGenderId; } set { _member.FGenderId = value; }
		}

		[Display(Name = "手機")]
		public string? Cellphone
		{
			get { return _member.FCellphone; } set { _member.FCellphone = value; }
		}

        //[Display(Name = "縣市")]
        [Required(ErrorMessage = "請選擇縣市")]
        public int? City { get; set; }

		//[Display(Name = "區域")]
		public int? Area
		{
			get { return _member.FAreaId; } set { _member.FAreaId = value; }
		}

		[Display(Name = "住址")]
		public string? Address
		{
			get { return _member.FAddress; } set { _member.FAddress = value; }
		}

        [Display(Name = "會員大頭貼")]
        public string? PhotoPath { get { return _member.FPhotoPath; } set { _member.FPhotoPath = value; } }

		[Display(Name = "個人簡介")]
        [MaxLength(500, ErrorMessage = "個人簡介不能超過{0}字")]
        public string? Introduction { get { return _member.FIntroduction; } set { _member.FIntroduction = value; } }

        [Display(Name = "上傳新大頭貼照")]
        public IFormFile? MemberPhoto { get; set; }

	}
}
