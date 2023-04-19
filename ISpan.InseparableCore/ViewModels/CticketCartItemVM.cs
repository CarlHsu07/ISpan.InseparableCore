using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
namespace ISpan.InseparableCore.ViewModels
{
    public class CticketCartItemVM
    {
        private TicketOrderCreateDto _ticketOrderDetails;
        public TicketOrderCreateDto ticket
        {
            get { return _ticketOrderDetails; }
            set { _ticketOrderDetails = value; }
        }
        public CticketCartItemVM()
        {
            _ticketOrderDetails = new TicketOrderCreateDto();
        }
        public int FId { get { return _ticketOrderDetails.FId; } set { _ticketOrderDetails.FId = value; } }
        public int FTicketItemNo { get { return _ticketOrderDetails.FTicketItemNo; } set { _ticketOrderDetails.FTicketItemNo = value; } }
        public int FOrderId { get { return _ticketOrderDetails.FOrderId; } set { _ticketOrderDetails.FOrderId = value; } }
        public int FMovieId { get { return _ticketOrderDetails.FMovieId; } set { _ticketOrderDetails.FMovieId = value; } }
        public string FMovieName { get { return _ticketOrderDetails.FMovieName; } set { _ticketOrderDetails.FMovieName = value; } }
        public int FSessionId { get { return _ticketOrderDetails.FSessionId; } set { _ticketOrderDetails.FSessionId = value; } }
        public int FSeatId { get { return _ticketOrderDetails.FSeatId; } set { _ticketOrderDetails.FSeatId = value; } }
        public int FRoomId { get { return _ticketOrderDetails.FRoomId; } set { _ticketOrderDetails.FRoomId = value; } }
        public decimal FTicketUnitprice { get { return _ticketOrderDetails.FTicketUnitprice; } set { _ticketOrderDetails.FTicketUnitprice = value; } }
        public decimal FTicketDiscount { get { return _ticketOrderDetails.FTicketDiscount; } set { _ticketOrderDetails.FTicketDiscount = value; } }
        public decimal FTicketSubtotal { get { return _ticketOrderDetails.FTicketSubtotal; }set { _ticketOrderDetails.FTicketSubtotal = value; } }
        public bool Fstatus { get { return _ticketOrderDetails.Fstatus; } set { _ticketOrderDetails.Fstatus = value; } }
    }
}
