using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
	public class CEditProfileViewModel
	{
		private TMembers _member;

		public TMembers member
		{
			get { return _member; }
			set { _member = value; }
		}

		public CEditProfileViewModel()
		{
			_member = new TMembers();
		}

		//[Display(Name = "會員流水號")]
		public int Id
		{
			get { return _member.FId; }
			set { _member.FId = value; }
		}

		[Display(Name = "會員編號")]
		public string MemberId
		{
			get { return _member.FMemberId; }
			set { _member.FMemberId = value; }
		}

		[Display(Name = "姓氏")]
		[Required(ErrorMessage = "請輸入姓氏")]
		public string LastName
		{
			get { return _member.FLastName; }
			set { _member.FLastName = value; }
		}

		[Display(Name = "名字")]
		[Required(ErrorMessage = "請輸入名字")]
		public string FirstName
		{
			get { return _member.FFirstName; }
			set { _member.FFirstName = value; }
		}

		[Display(Name = "Email")]
		[Required(ErrorMessage = "請輸入Email")]
		public string Email
		{
			get { return _member.FEmail; }
			set { _member.FEmail = value; }
		}

		[Display(Name = "密碼")]
		[Required(ErrorMessage = "請輸入密碼")]
		[DataType(DataType.Password)]
		public string Password
		{
			get { return null; }
			set { _member.FPasswordHash = value; }
		}

		[Display(Name = "確認密碼")]
		[Required(ErrorMessage = "請再次輸入密碼")]
		[Compare("Password", ErrorMessage = "密碼不相同！")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }

		[Display(Name = "生日")]
		public DateTime? DateOfBirth
		{
			get { return _member.FDateOfBirth; }
			set { _member.FDateOfBirth = value; }
		}

		[Display(Name = "性別")]
		public int? GenderId
		{
			get { return _member.FGenderId; }
			set { _member.FGenderId = value; }
		}

		[Display(Name = "手機")]
		public string? Cellphone
		{
			get { return _member.FCellphone; }
			set { _member.FCellphone = value; }
		}

		//[Display(Name = "縣市")]
		[Required(ErrorMessage = "請選擇縣市")]
		public int? City { get; set; }

		//[Display(Name = "區域")]
		[Required(ErrorMessage = "請選擇區域")]
		public int? Area
		{
			get { return _member.FAreaId; }
			set { _member.FAreaId = value; }
		}

		[Display(Name = "住址")]
		public string? Address
		{
			get { return _member.FAddress; }
			set { _member.FAddress = value; }
		}

		public string? PhotoPath
		{
			get { return _member.FPhotoPath; }
			set { _member.FPhotoPath = value; }
		}

		[Display(Name = "自我介紹")]
		public string? Introduction
		{
			get { return _member.FIntroduction; }
			set { _member.FIntroduction = value; }
		}

		//[Display(Name = "會員狀態")]
		//public int? AccountStatus
		//{
		//    get { return _member.FAccountStatus; }
		//    set { _member.FAccountStatus = value; }
		//}

		//[Display(Name = "會員點數")]
		//public int? TotalMemberPoint
		//{
		//    get { return _member.FTotalMemberPoint; }
		//    set { _member.FTotalMemberPoint = value; }
		//}

		//[Display(Name = "註冊時間")]
		//public DateTime? SignUpTime
		//{
		//	get { return _member.FSignUpTime; }
		//	set { _member.FSignUpTime = value; }
		//}

		public IFormFile photo { get; set; }

	}
}
