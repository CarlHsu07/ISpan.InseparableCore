using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.ViewModels
{
    public class CcartviewVM
    {
        public List<CproductCartItem> cart { get; set; }
        public Dictionary<int, string> seats { get; set; }
        public int regular { get; set; }
        public int concession { get; set; }
        public TSessions session { get; set; }
        public IEnumerable<TMovies> movies { get; set; }
    }
}
