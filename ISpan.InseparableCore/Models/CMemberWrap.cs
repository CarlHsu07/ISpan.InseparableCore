using ISpan.InseparableCore.Models;

namespace ISpan.InseparableCoreMVC.Models
{
    public class CMemberWrap
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

        public int FId
        {
            get { return _member.FId; }
            set { _member.FId = value; }
        }

        public string FLastName
        {
            get { return _member.FLastName; }
            set { _member.FLastName = value; }
        }

        public string FFirstName
        {
            get { return _member.FFirstName; }
            set { _member.FFirstName = value; }
        }

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

        public DateTime? FDateOfBirth
        {
            get { return _member.FDateOfBirth; }
            set { _member.FDateOfBirth = value; }
        }

        public int? FGender
        {
            get { return _member.FGenderId; }
            set { _member.FGenderId = value; }
        }

        public string? FCellphone
        {
            get { return _member.FCellphone; }
            set { _member.FCellphone = value; }
        }

        public string? FAddress
        {
            get { return _member.FAddress; }
            set { _member.FAddress = value; }
        }

        public string? FPhotoPath
        {
            get { return _member.FPhotoPath; }
            set { _member.FPhotoPath = value; }
        }

        public string? FIntroduction
        {
            get { return _member.FIntroduction; }
            set { _member.FIntroduction = value; }
        }

        public IFormFile photo { get; set; }
    }
}
