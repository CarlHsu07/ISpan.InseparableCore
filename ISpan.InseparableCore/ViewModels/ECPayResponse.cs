using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ISpan.InseparableCore.ViewModels
{
    public class ECPayResponse
    {
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public int RtnCode { get; set; }
        public string RtnMsg { get; set; }
        public string TradeNo { get; set; }
        public int TradeAmt { get; set; }
        public string PaymentDate { get; set; }
        public string TradeDate { get; set; }
        public int SimulatePaid { get; set; }
    }
}
