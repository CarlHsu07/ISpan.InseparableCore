namespace ISpan.InseparableCore.ViewModels
{
    public class CticketCartItemVM
    {
         
        public int FId { get; set; }
        public int FTicketItemNo { get; set; }
        public int FOrderId { get; set; }
        public int FMovieId { get; set; }
        public int FSessionId { get; set; }
        public int FSeatId { get; set; }
        public int FRoomId { get; set; }
        public decimal FTicketUnitprice { get; set; }
        public decimal FTicketDiscount { get; set; }
        public decimal FTicketSubtotal { get; set; }
    }
}
