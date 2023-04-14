using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.ViewModels
{
    public class CbookingVM
    {
        public IEnumerable<TCinemas> cinema { get; set; }
        public IEnumerable<TMovies> movie { get; set; }
        public IEnumerable<TSessions> sessions { get; set; }
        public IEnumerable<TProducts> products { get; set; }
        //public int ticket { get; set; }
    }
}
