using Microsoft.Data.Sqlite;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class OrderRepository
    {
        private readonly InseparableContext _db;
        public OrderRepository(InseparableContext db)
        {
            _db = db;
        }
        public void Create(TOrders orders)
        {
            try
            {
                _db.TOrders.Add(orders);
                _db.SaveChanges();
            }
            catch (SqliteException ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public TOrders GetByAll(TOrders orders)
        {
            var data = _db.TOrders.FirstOrDefault(t => t == orders);
            return data;
        }
        public TOrders GetById(int? id)
        {
            var data = _db.TOrders.FirstOrDefault(t => t.FOrderId == id);
            return data;
        }
    }
}
