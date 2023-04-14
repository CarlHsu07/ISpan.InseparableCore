using ISpan.InseparableCore.Models.DAL;
namespace ISpan.InseparableCore.ViewModels
{
    public class CproductCartItem
    {
        private TProductOrderDetails _productOrderDetails;
        public TProductOrderDetails ProductOrderDetails
        {
            get { return _productOrderDetails; }
            set { _productOrderDetails = value; }
        }
        public CproductCartItem()
        {
            _productOrderDetails = new TProductOrderDetails();
        }
        public int FId { get { return _productOrderDetails.FId; } set { _productOrderDetails.FId = value; } }
        public int FProductItemNo { get { return _productOrderDetails.FProductItemNo; } set { _productOrderDetails.FProductItemNo = value; } }
        public int FOrderId { get { return _productOrderDetails.FOrderId; } set { _productOrderDetails.FOrderId = value; } }
        public int FProductId { get { return _productOrderDetails.FProductId; } set { _productOrderDetails.FProductId = value; } }
        public string FProductName { get { return _productOrderDetails.FProductName; } set { _productOrderDetails.FProductName = value; } }
        public decimal FProductUnitprice { get { return _productOrderDetails.FProductUnitprice; } set { _productOrderDetails.FProductUnitprice = value; } }
        public int FProductQty { get { return _productOrderDetails.FProductQty; } set { _productOrderDetails.FProductQty = value; } }
        public decimal FProductDiscount { get { return _productOrderDetails.FProductDiscount; } set { _productOrderDetails.FProductDiscount = value; } }
        public decimal FProductSubtotal { get { return _productOrderDetails.FProductSubtotal; }set { _productOrderDetails.FProductSubtotal = value; } }


    }
}
