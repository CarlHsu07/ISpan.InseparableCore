using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
    public class CChangePasswordViewModel
    {
        //[Display(Name = "會員流水號")]
        public int Id { get; set; }

        [Display(Name = "目前密碼")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "密碼長度至少為 {2} 個字元。", MinimumLength = 8)]
        [Required(ErrorMessage = "請輸入目前密碼")]
        public string CurrentPassword { get; set; }

        [Display(Name = "新密碼")]
        [Required(ErrorMessage = "請輸入新密碼")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+-=,./?;:'""[\]{}\\|]{8,}$", ErrorMessage = "密碼至少包含一個英文、一個數字，長度至少為8個字元。")]
        public string NewPassword { get; set; }

        [Display(Name = "確認新密碼")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "密碼不相同！")]
        [Required(ErrorMessage = "請再次輸入新密碼")]
        public string ConfirmPassword { get; set; }
    }
}
