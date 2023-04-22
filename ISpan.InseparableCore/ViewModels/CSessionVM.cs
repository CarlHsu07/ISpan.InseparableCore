using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
    public class CSessionVM
    {
        private TSessions _sessions;
        public TSessions session { get { return _sessions; } set { _sessions = value; } }
        public CSessionVM()
        {
            _sessions = new TSessions();
        }
        public int FSessionId { get { return _sessions.FSessionId; } set { _sessions.FSessionId = value; } }
        [Display(Name = "電影")]
        public int FMovieId { get { return _sessions.FMovieId; } set { _sessions.FMovieId = value; } }
        [Display(Name = "影廳")]
        public int FRoomId { get { return _sessions.FRoomId; } set { _sessions.FRoomId = value; } }
        [Display(Name = "影城")]
        public int FCinemaId { get { return _sessions.FCinemaId; } set { _sessions.FCinemaId = value; } }
        [Display(Name = "日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime FSessionDate { get { return _sessions.FSessionDate; } set { _sessions.FSessionDate = value; } }
        [Display(Name = "時間")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan FSessionTime { get { return _sessions.FSessionTime; } set { _sessions.FSessionTime = value; } }
        [Display(Name = "票價")]
        public decimal FTicketPrice { get { return _sessions.FTicketPrice; } set { _sessions.FTicketPrice = value; } }
        [Display(Name = "影城")]
        public virtual TCinemas FCinema { get; set; }
        [Display(Name = "電影")]
        public virtual TMovies FMovie { get; set; }
        [Display(Name = "影廳")]
        public virtual TRooms FRoom { get; set; }
        [Display(Name = "影城")]
        public string FMovieName { get => FMovie.FMovieName; set => FMovie.FMovieName = value; }
        [Display(Name = "影城")]
        public string FCinemaName { get => FCinema.FCinemaName; set => FCinema.FCinemaName = value; }
        [Display(Name = "影廳")]
        public string FRoomName { get => FRoom.FRoomName; set => FRoom.FRoomName = value; }
    }
}
