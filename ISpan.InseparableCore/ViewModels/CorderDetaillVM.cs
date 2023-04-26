using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
    public class CorderDetaillVM
    {
        private TOrders _orders;
        public TOrders orders
        {
            get { return _orders; }
            set { _orders = value; }
        }
        public CorderDetaillVM()
        {
            _orders = new TOrders();
        }
        [Display(Name = "訂單編號")]
        public int FOrderId { get { return _orders.FOrderId; } set { _orders.FOrderId = value; } }
        [Display(Name = "會員編號")]
        public int FMemberId { get { return _orders.FMemberId; } set { _orders.FMemberId = value; } }
        [Display(Name = "影院")]
        public int FCinemaId { get { return _orders.FCinemaId; } set { _orders.FCinemaId = value; } }
        [Display(Name = "訂購日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime FOrderDate { get { return _orders.FOrderDate; } set { _orders.FOrderDate = value; } }
        [Display(Name = "異動日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime FModifiedTime { get { return _orders.FModifiedTime; } set { _orders.FModifiedTime = value; } }
        [Display(Name = "總金額")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal FTotalMoney { get { return _orders.FTotalMoney; } set { _orders.FTotalMoney = value; } }
        [Display(Name = "訂單狀態")]
        public bool FStatus { get { return _orders.FStatus; } set { _orders.FStatus = value; } }
        [Display(Name = "刷卡編號")]
        public string FCreditTradeNo { get { return _orders.FCreditTradeNo; } set { _orders.FCreditTradeNo = value; } }
        public int regular { get; set; }
        public int concession { get; set; }
        [Display(Name = "影院")]
        public virtual TCinemas FCinema { get; set; }
        [Display(Name = "會員編號")]
        public virtual TMembers FMember { get; set; }
        public string Cinema { get=>FCinema.FCinemaName; set=>FCinema.FCinemaName=value; }
        public string MemberNo { get=>FMember.FMemberId; set=>FMember.FMemberId=value; }
        public IEnumerable<TTicketOrderDetails> ticket { get; set; }
        public IEnumerable<TProductOrderDetails> product { get; set; }
        
    }
}
