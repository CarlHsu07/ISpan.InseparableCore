using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
    public class CAdminLoginViewModel
    {
        private TAdministrators _administrator;

        public TAdministrators administrator
        {
            get { return _administrator; }
            set { _administrator = value; }
        }

        public CAdminLoginViewModel()
        {
            _administrator = new TAdministrators();
        }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "請輸入正確的Email格式")]
        [Required(ErrorMessage = "請輸入Email")]
        public string Email
        {
            get { return _administrator.FEmail; }
            set { _administrator.FEmail = value; }
        }

        [Display(Name = "密碼")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "密碼長度至少為 {2} 個字元。", MinimumLength = 4)]
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password
        {
            get { return _administrator.FPasswordHash; }
            set { _administrator.FPasswordHash = value; }
        }

    }
}
