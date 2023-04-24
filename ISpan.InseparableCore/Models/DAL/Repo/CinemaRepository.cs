using ISpan.InseparableCore.ViewModels;
using ISpan.InseparableCore.ViewModels.TCinemasVM;
using Microsoft.Extensions.Options;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class CinemaRepository
    {
        private readonly InseparableContext _db;
        public CinemaRepository(InseparableContext db)
        {
            _db = db;
        }
        public IEnumerable<TCinemas> QueryAll()
        {
            var data = _db.TCinemas.Select(t => t);
            return data;
        }
        public IEnumerable<TCinemas> GetByCity(string city)
        {
            var data = _db.TCinemas.Where(t => t.FCinemaRegion == city);
            return data;
        }
        public IEnumerable<TCinemas> GetByBrand(string brand)
        {
            var data = _db.TCinemas.Where(t => t.FCinemaName.Contains(brand));
            return data;
        }
        public List<CTCinemasVM> CinemaSearch(CTCinemaSearch search)
        {
            List<CTCinemasVM> vm = new List<CTCinemasVM>();
            var data = QueryAll();

            if (search != null)
            {
                if (search.city != null && search.city!= "請選擇")
                    data = data.Where(t => t.FCinemaRegion == search.city);
                if (search.brand != null && search.brand != "請選擇")
                    data = data.Where(t => t.FCinemaName.Contains(search.brand));
            }

            foreach(var item in data)
            {
                CTCinemasVM cinema = new CTCinemasVM();
                cinema.cinemas = item;

                vm.Add(cinema);

            }
            return vm;
        }
        public CTCinemasVM GetCinema(int? id)
        {
            if (id == null)
                return null;

            var data = _db.TCinemas.FirstOrDefault(t => t.FCinemaId == id);

            return data;
        }
    }
}
