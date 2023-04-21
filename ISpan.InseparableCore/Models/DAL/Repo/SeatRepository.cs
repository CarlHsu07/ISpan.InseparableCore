namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class SeatRepository
    {
        private readonly InseparableContext _db;
        public SeatRepository(InseparableContext db)
        {
            _db = db;
        }
        public IEnumerable<TSeats> GetSeat()
        {
            var data = _db.TSeats.Select(t => t);
            return data;
        }
    }
}
