using ISpan.InseparableCore.Models.BLL.Core;
using ISpan.InseparableCore.Models.BLL.Interface;
using ISpan.InseparableCore.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class SessionRepository:ISessionRepository
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
            if (cinema == null)
                return null;

            var data = _db.TSessions.Where(t => t.FCinemaId == cinema && t.FSessionDate >= start && t.FSessionDate <= end).Select(t => t.FMovie).Distinct();

            return data;
        }
        public IEnumerable<TSessions> GetSessionByTwoCondition(int? cinema, int? movie)
        {
            if (cinema == null || movie == null)
                return null;

            var data = _db.TSessions.Where(t => t.FCinemaId == cinema && t.FMovieId == movie && t.FSessionDate >= start && t.FSessionDate <= end);

            return data;
        }
        public IEnumerable<TSessions> GetSessionBySession(int? session)
        {
            if (session == null)
                return null;

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
            var today = DateTime.Now.Date;
            List<CSessionVM> data = new List<CSessionVM>();
            var inseparableContext = _db.TSessions.OrderByDescending(t => t.FSessionDate).Include(t => t.FCinema).Include(t => t.FMovie).Include(t => t.FRoom).Where(t=>t.FSessionDate>=today);

            if (item != null)
            {
                if (item.cinema != 0)
                    inseparableContext = inseparableContext.Where(t => t.FCinemaId == item.cinema);
                if (item.movie != 0)
                    inseparableContext = inseparableContext.Where(t => t.FMovieId == item.movie);
            }
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
        public void Create(SessionEntity entity)
        {
            try
            {
                _db.TSessions.Add(entity.session);
                _db.SaveChanges();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Edit(SessionEntity entity)
        {
            var edit = _db.TSessions.FirstOrDefault(t => t.FSessionId == entity.FSessionId);
            edit.FRoomId = entity.FRoomId;
            edit.FSessionDate = entity.FSessionDate;
            edit.FSessionTime = entity.FSessionTime;
            edit.FMovieId = entity.FMovieId;
            edit.FTicketPrice = entity.FTicketPrice;

            try
            {
                _db.SaveChanges();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public TSessions GetByDateTime(int? room, DateTime date, TimeSpan time)
        {
            var data = _db.TSessions.FirstOrDefault(t => t.FRoomId == room && t.FSessionDate == date && t.FSessionTime == time);

            return data;
        }
        public CSessionVM GetSession(int? id)
        {
            if (id == null)
                return null;

            var data =  _db.TSessions
                .Include(t => t.FCinema)
                .Include(t => t.FMovie)
                .Include(t => t.FRoom)
                .FirstOrDefault(m => m.FSessionId == id);

            if (data == null)
                return null;

            CSessionVM vm = new CSessionVM();
            vm.session = data;
            vm.FMovie = data.FMovie;
            vm.FCinema=data.FCinema;
            vm.FRoom= data.FRoom;
            return vm;
        }
       public void Delete(TSessions sessions)
        {
            try
            {
                _db.Remove(sessions);
                _db.SaveChanges();
            }catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
