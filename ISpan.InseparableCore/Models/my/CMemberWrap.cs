using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.Models.my
{
    public class CMemberWrap // todo 不使用，改用ViewModel
    {
        private TMembers _member;

        public TMembers member
        {
            get { return _member; }
            set { _member = value; }
        }

        public CMemberWrap()
        {
            _member = new TMembers();
        }

        [Display(Name = "會員流水號")]
        public int FId
        {
            get { return _member.FId; }
            set { _member.FId = value; }
        }

        [Display(Name = "會員ID")]
        public string FMemberId
        {
            get { return _member.FMemberId; }
            set { _member.FMemberId = value; }
        }

        [Display(Name = "姓氏")]
        public string FLastName
        {
            get { return _member.FLastName; }
            set { _member.FLastName = value; }
        }

        [Display(Name = "名字")]
        public string FFirstName
        {
            get { return _member.FFirstName; }
            set { _member.FFirstName = value; }
        }

        [Display(Name = "Email")]
        public string FEmail
        {
            get { return _member.FEmail; }
            set { _member.FEmail = value; }
        }

        public string FPasswordHash
        {
            get { return _member.FPasswordHash; }
            set { _member.FPasswordHash = value; }
        }

        public string FPasswordSalt
        {
            get { return _member.FPasswordSalt; }
            set { _member.FPasswordSalt = value; }
        }

        [Display(Name = "生日")]
        public DateTime? FDateOfBirth
        {
            get { return _member.FDateOfBirth; }
            set { _member.FDateOfBirth = value; }
        }

        [Display(Name = "性別")]
        public int? FGenderId
        {
            get { return _member.FGenderId; }
            set { _member.FGenderId = value; }
        }

        [Display(Name = "手機")]
        public string? FCellphone
        {
            get { return _member.FCellphone; }
            set { _member.FCellphone = value; }
        }

        [Display(Name = "住址")]
        public string? FAddress
        {
            get { return _member.FAddress; }
            set { _member.FAddress = value; }
        }

        [Display(Name = "居住地區")]
        public int? FAreaZipCode
        {
            get { return _member.FAreaZipCode; }
            set { _member.FAreaZipCode = value; }
        }


        public string? FPhotoPath
        {
            get { return _member.FPhotoPath; }
            set { _member.FPhotoPath = value; }
        }

        [Display(Name = "自我介紹")]
        public string? FIntroduction
        {
            get { return _member.FIntroduction; }
            set { _member.FIntroduction = value; }
        }

        [Display(Name = "會員狀態")]
        public int? FAccountStatus
        {
            get { return _member.FAccountStatus; }
            set { _member.FAccountStatus = value; }
        }

        [Display(Name = "會員點數")]
        public int? FTotalMemberPoint
        {
            get { return _member.FTotalMemberPoint; }
            set { _member.FTotalMemberPoint = value; }
        }

        [Display(Name = "註冊時間")]
        public DateTime? FSignUpTime
        {
            get { return _member.FSignUpTime; }
            set { _member.FSignUpTime = value; }
        }

        public IFormFile photo { get; set; }
    }
}
