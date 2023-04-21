using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.BLL.Interfaces;
using Microsoft.Data.Sqlite;

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
            var data = _db.TTicketOrderDetails.Where(t => t.FSessionId == seesionid && t.FStatus == status);
            return data;
        }
        public IEnumerable<TTicketOrderDetails> GetById(int? id)
        {
            var data = _db.TTicketOrderDetails.Where(t => t.FOrderId == id);
            return data;
        }
        public TTicketOrderDetails GetBySeat(int? seesionid, bool status, int? seat)
        {
            var data = _db.TTicketOrderDetails.Where(t => t.FSessionId == seesionid && t.FStatus == status).FirstOrDefault(t => t.FSeatId == seat);
            return data;
        }
    }
}
