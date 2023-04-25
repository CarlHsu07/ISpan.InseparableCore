namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class RoomRepository
    {
        private readonly InseparableContext _db;
        public RoomRepository(InseparableContext db)
        {
            _db = db;
        }
        public IEnumerable<TRooms> GetByCinema(int? cinema)
        {
            if (cinema == null)
                return null;

            var data = _db.TRooms.Where(t => t.FCinemaId == cinema);

            return data;
        }
    }
}
