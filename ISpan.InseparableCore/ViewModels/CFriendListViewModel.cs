using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
    public class CFriendListViewModel
    {
        private TMembers _member;

        public TMembers member
        {
            get { return _member; }
            set { _member = value; }
        }

        public CFriendListViewModel()
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
        public string? MemberId
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

        [Display(Name = "會員大頭貼")]
        public string? PhotoPath
        {
            get { return _member.FPhotoPath; }
            set { _member.FPhotoPath = value; }
        }
    }
}
