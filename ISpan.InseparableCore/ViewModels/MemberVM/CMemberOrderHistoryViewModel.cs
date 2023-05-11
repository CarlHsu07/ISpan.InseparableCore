using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;
using static NuGet.Packaging.PackagingConstants;

namespace ISpan.InseparableCore.ViewModels.MemberVM
{
    public class CMemberOrderHistoryViewModel
    {
        private TOrders _order;
        public TOrders order
        {
            get { return _order; }
            set { _order = value; }
        }
        public CMemberOrderHistoryViewModel()
        {
            _order = new TOrders();
        }

        [Display(Name = "訂單編號")]
        public int OrderId { get { return _order.FOrderId; } set { _order.FOrderId = value; } }

        [Display(Name = "影城")]
        public int CinemaId { get { return _order.FCinemaId; } set { _order.FCinemaId = value; } }

        [Display(Name = "訂購時間")]
        public DateTime OrderDate { get { return _order.FOrderDate; } set { _order.FOrderDate = value; } }

        [Display(Name = "修改時間")]
        public DateTime ModifiedTime { get { return _order.FModifiedTime; } set { _order.FModifiedTime = value; } }

        [Display(Name = "總金額")]
        public decimal TotalMoney { get { return _order.FTotalMoney; } set { _order.FTotalMoney = value; } }

        [Display(Name = "訂單狀態")]
        public bool Status { get { return _order.FStatus; } set { _order.FStatus = value; } }
    }
}
