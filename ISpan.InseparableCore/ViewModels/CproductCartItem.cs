using ISpan.InseparableCore.Models;

namespace ISpan.InseparableCore.ViewModels
{
    public class CproductCartItem
    {
        public int FId { get; set; }
        public int FProductItemNo { get; set; }
        //public int FOrderId { get; set; }
        public int FProductId { get; set; }
        public string FProductName { get; set; }
        public decimal FProductUnitprice { get; set; }
        public int FProductQty { get; set; }
        //public decimal FProductDiscount { get; set; }
        public decimal FProductSubtotal { get { return this.FProductQty * this.FProductUnitprice; } }

        public  TOrders FOrder { get; set; }
        public  TProducts FProduct { get; set; }

    }
}
