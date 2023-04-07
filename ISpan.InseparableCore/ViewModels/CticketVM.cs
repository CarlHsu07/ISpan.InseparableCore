using ISpan.InseparableCore.Models;

namespace ISpan.InseparableCore.ViewModels
{
    public class CticketVM
    {
        public IEnumerable<TCinemas> cinema { get; set; }
        public IEnumerable<TMovies> movie { get; set; }
        public Dictionary<DateTime, IEnumerable<TSessions>> sessions { get; set; }

        public int? cinemaId { get; set; }
        public int? movieId { get; set; }
        public int? sessionId { get; set; }
    }
}
