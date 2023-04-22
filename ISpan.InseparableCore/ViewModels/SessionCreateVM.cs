using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.BLL.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
    public class SessionCreateVM
    {
        private SessionCreateDto _sessions;
        public SessionCreateDto session { get { return _sessions; } set { _sessions = value; } }
        public SessionCreateVM()
        {
            _sessions = new SessionCreateDto();
        }
        public int FSessionId { get { return _sessions.FSessionId; } set { _sessions.FSessionId = value; } }
        [Display(Name = "電影")]
        [Required(ErrorMessage = "{0}必填")]
        public int FMovieId { get { return _sessions.FMovieId; } set { _sessions.FMovieId = value; } }
        [Display(Name = "影廳")]
        [Required(ErrorMessage = "{0}必填")]
        public int FRoomId { get { return _sessions.FRoomId; } set { _sessions.FRoomId = value; } }
        [Display(Name = "影城")]
        [Required(ErrorMessage = "{0}必填")]
        public int FCinemaId { get { return _sessions.FCinemaId; } set { _sessions.FCinemaId = value; } }
        [Display(Name = "日期")]
        [Required(ErrorMessage = "{0}必填")]
        public DateTime FSessionDate { get { return _sessions.FSessionDate; } set { _sessions.FSessionDate = value; } }
        [Display(Name = "時間")]
        [Required(ErrorMessage = "{0}必填")]
        public TimeSpan FSessionTime { get { return _sessions.FSessionTime; } set { _sessions.FSessionTime = value; } }
        [Display(Name = "票價")]
        [Required(ErrorMessage = "{0}必填")]
        public decimal FTicketPrice { get { return _sessions.FTicketPrice; } set { _sessions.FTicketPrice = value; } }

    }
}
