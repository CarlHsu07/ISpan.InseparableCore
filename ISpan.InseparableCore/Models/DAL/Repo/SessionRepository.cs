using ISpan.InseparableCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class SessionRepository
    {
        private readonly InseparableContext _db;
        public SessionRepository(InseparableContext db)
        {
            _db = db;
        }
        //限制時間區間
        public DateTime start = DateTime.Now.Date;
        public DateTime end = DateTime.Now.Date.AddDays(7);
        public IEnumerable<TMovies> GetMovieByCinema(int? cinema)
        {
            var data = _db.TSessions.Where(t => t.FCinemaId == cinema && t.FSessionDate >= start && t.FSessionDate <= end).Select(t => t.FMovie).Distinct();

            return data;
        }
        public IEnumerable<TSessions> GetSession(int? cinema, int? movie)
        {
            var data = _db.TSessions.Where(t => t.FCinemaId == cinema && t.FMovieId == movie && t.FSessionDate >= start && t.FSessionDate <= end);

            return data;
        }
        public IEnumerable<TSessions> GetSessionBySession(int? session)
        {
            var data = _db.TSessions.Where(t => t.FSessionId == session);

            return data;
        }
        public TSessions GetOneSession(int? session)
        {
            var data = _db.TSessions.FirstOrDefault(t => t.FSessionId == session);

            return data;
        }
        public IEnumerable<TMovies> GetMovieBySEssion(int? session)
        {
            var data = _db.TSessions.Where(t => t.FSessionId == session).Select(t => t.FMovie);
            return data;
        }
        public IEnumerable<TCinemas> GetCinemaBySEssion(int? session)
        {
            var data = _db.TSessions.Where(t => t.FSessionId == session).Select(t => t.FCinema);
            return data;
        }
        public List<CSessionVM> SessionSearch(CSessionSearch item)
        {
            List<CSessionVM> data = new List<CSessionVM>();
            var inseparableContext = _db.TSessions.Select(t => t); ;

            if (item.cinema != null)
                inseparableContext = inseparableContext.Where(t => t.FCinemaId == item.cinema);
            if (item.movie != null)
                inseparableContext = inseparableContext.Where(t => t.FMovieId == item.movie);

            inseparableContext = inseparableContext.OrderByDescending(t => t.FSessionDate).Include(t => t.FCinema).Include(t => t.FMovie).Include(t => t.FRoom);
            foreach(var value in inseparableContext)
            {
                CSessionVM vm = new CSessionVM();
                vm.session = value;
                vm.FMovie = value.FMovie;
                vm.FCinema = value.FCinema;
                vm.FRoom = value.FRoom;

                data.Add(vm);
            }
            return data;
        }
    }
}
