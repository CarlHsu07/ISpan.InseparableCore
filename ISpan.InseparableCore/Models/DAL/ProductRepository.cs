namespace ISpan.InseparableCore.Models.DAL
{
    public class ProductRepository
    {
        private readonly InseparableContext _db;
        public ProductRepository(InseparableContext db)
        {
            _db= db;
        }
        public IEnumerable<TProducts> GetProductByCinema(int? cinema)
        {
            var data = _db.TProducts.Where(t => t.FCinemaId == cinema);
            return data;
        }
        public TProducts GetOneProduct(int? product)
        {
            var data = _db.TProducts.FirstOrDefault(t => t.FProductId == product);
            return data;
        }
    }
}
