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
    }
}
