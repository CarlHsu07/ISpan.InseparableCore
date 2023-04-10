using ISpan.InseparableCore.Models;

namespace ISpan.InseparableCore.ViewModels
{
    public class CseatVM
    {
        public int regularnum { get; set; }
        public int concessionnum { get; set; }
        public int? sessionid { get; set; }
        public Dictionary<string, IEnumerable<TSeats>> seats { get; set; }
        public List<int> solid { get; set; }
        public List<int> myseats { get; set; }
        public int ticket { get { return this.regularnum + this.concessionnum; } }

        public TSessions sessions { get; set; }
        public IEnumerable<TMovies> movie { get; set; }
    }
}
