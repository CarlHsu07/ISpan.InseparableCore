using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.BLL.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class TicketOrderRepository : ITicketOrderRepository
    {
        private readonly InseparableContext _db;
        public TicketOrderRepository(InseparableContext db)
        {
            _db = db;
        }
        public void Create(TicketOrderEntity entity)
        {
            if (entity == null) throw new Exception("資料缺失!!");
            try
            {
                _db.TTicketOrderDetails.Add(entity.ticket);
                _db.SaveChanges();
            }
            catch (SqliteException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<TTicketOrderDetails> GetSolid(int? seesionid, bool status)
        {
            if (seesionid == null || status == null)
                return null;

            var data = _db.TTicketOrderDetails.Where(t => t.FSessionId == seesionid && t.FStatus == status);

            return data;
        }
        public IEnumerable<TTicketOrderDetails> GetById(int? id)
        {
            if (id == null)
                return null;

            var data = _db.TTicketOrderDetails.Where(t => t.FOrderId == id);

            return data;
        }
        public TTicketOrderDetails GetBySeat(int? seesionid, bool status, int? seat)
        {
            if (seesionid == null || status == null || seat == null)
                return null;
            
            var data = _db.TTicketOrderDetails.Where(t => t.FSessionId == seesionid && t.FStatus == status).FirstOrDefault(t => t.FSeatId == seat);

            return data;
        }
    }
}
