namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class ProductRepository
    {
        private readonly InseparableContext _db;
        public ProductRepository(InseparableContext db)
        {
            _db = db;
        }
        public IEnumerable<TProducts> GetProductByCinema(int? cinema)
        {
            if (cinema == null)
                return null;

            var data = _db.TProducts.Where(t => t.FCinemaId == cinema);
            return data;
        }
        public TProducts GetOneProduct(int? product)
        {
            if (product == null)
                return null;

            var data = _db.TProducts.FirstOrDefault(t => t.FProductId == product);
            return data;
        }
    }
}
