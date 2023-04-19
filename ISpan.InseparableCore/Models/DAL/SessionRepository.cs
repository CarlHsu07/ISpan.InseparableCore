namespace ISpan.InseparableCore.Models.DAL
{
    public class SessionRepository
    {
        private readonly InseparableContext _db;
        public SessionRepository(InseparableContext db)
        {
            _db = db;
        }
        public IEnumerable<TMovies> GetMovie(int? cinema)
        {
            var data = _db.TSessions.Where(t => t.FCinemaId ==cinema).Select(t => t.FMovie).Distinct();
            // &&t.FSessionDate>=start && t.FSessionDate<=end 

            return data;
        }
        public IEnumerable<TSessions> GetSession(int? cinema,int? movie)
        {
            var data = _db.TSessions.Where(t => t.FCinemaId == cinema && t.FMovieId == movie);
            // &&t.FSessionDate>=start && t.FSessionDate<=end 

            return data;
        }
        public IEnumerable<TSessions> GetBySession(int? session)
        {
            var data = _db.TSessions.Where(t => t.FSessionId == session);

            return data;
        }
        public TSessions GetOneSession(int? session)
        {
            var data = _db.TSessions.FirstOrDefault(t => t.FSessionId == session);

            return data;
        }
    }
}
