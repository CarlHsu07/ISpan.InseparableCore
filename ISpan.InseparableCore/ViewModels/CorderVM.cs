using ISpan.InseparableCore.Models;
using System.Security.Cryptography.Xml;

namespace ISpan.InseparableCore.ViewModels
{
    public class CorderVM
    {
        private TOrders _orders;
        public TOrders orders
        {
            get { return _orders; }
            set { _orders = value; }
        }
        public CorderVM()
        {
            _orders = new TOrders();
        }
        public int FOrderId { get { return _orders.FOrderId; } set { _orders.FOrderId = value; } }
        public int FMemberId { get { return _orders.FMemberId; } set { _orders.FMemberId = value; } }
        public int FCinemaId { get { return _orders.FCinemaId; } set { _orders.FCinemaId = value; } }
        public DateTime FOrderDate { get { return _orders.FOrderDate; } set { _orders.FOrderDate = value; } }
        public DateTime FModifiedTime { get { return _orders.FModifiedTime; } set { _orders.FModifiedTime = value; } }
        public decimal FTotalMoney { get { return _orders.FTotalMoney; } set { _orders.FTotalMoney = value; } }
        public int regular { get; set; }
        public int concession { get; set; }
    }
}
