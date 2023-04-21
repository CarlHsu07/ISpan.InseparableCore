using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class ProductOrderRepository
    {
        private readonly InseparableContext _db;
        public ProductOrderRepository(InseparableContext db)
        {
            _db = db;
        }

        public void Create(TProductOrderDetails product)
        {
            try
            {
                _db.TProductOrderDetails.Add(product);
                _db.SaveChanges();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
