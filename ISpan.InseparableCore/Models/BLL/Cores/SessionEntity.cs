using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.Models.BLL.Core
{
    public class SessionEntity
    {
        private TSessions _sessions;
        public TSessions session { get { return _sessions; } set { _sessions = value; } }
        public SessionEntity()
        {
            _sessions = new TSessions();
        }
        public int FSessionId { get { return _sessions.FSessionId; } set { _sessions.FSessionId = value; } }
        public int FMovieId { get { return _sessions.FMovieId; } set { _sessions.FMovieId = value; } }
        public int FRoomId { get { return _sessions.FRoomId; } set { _sessions.FRoomId = value; } }
        public int FCinemaId { get { return _sessions.FCinemaId; } set { _sessions.FCinemaId = value; } }
        public DateTime FSessionDate { get { return _sessions.FSessionDate; } set { _sessions.FSessionDate = value; } }
        public TimeSpan FSessionTime { get { return _sessions.FSessionTime; } set { _sessions.FSessionTime = value; } }
        public decimal FTicketPrice { get { return _sessions.FTicketPrice; } set { _sessions.FTicketPrice = value; } }
    }
}
