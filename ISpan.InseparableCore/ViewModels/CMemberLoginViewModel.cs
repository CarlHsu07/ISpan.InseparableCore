using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
    public class CMemberLoginViewModel
    {
        private TMembers _member;

        public TMembers member
        {
            get { return _member; }
            set { _member = value; }
        }

        public CMemberLoginViewModel()
        {
            _member = new TMembers();
        }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "請輸入正確的Email格式")]
        [Required(ErrorMessage = "請輸入Email")]
        public string Email
        {
            get { return _member.FEmail; }
            set { _member.FEmail = value; }
        }

        [Display(Name = "密碼")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "密碼長度至少為 {2} 個字元。", MinimumLength = 8)]
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password
        {
            get { return _member.FPasswordHash; }
            set { _member.FPasswordHash = value; }
        }
    }
}
