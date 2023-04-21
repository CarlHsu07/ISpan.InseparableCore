using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ISpan.InseparableCore.Models.DAL
{
    public class MovieRepository
    {
        private readonly InseparableContext _db;
        public MovieRepository(InseparableContext db)
        {
            _db = db;
        }
        public TMovies GetOneMovie(int? movie)
        {
            var data = _db.TMovies.FirstOrDefault(t => t.FMovieId == movie);
            return data;
        }
    }
}
