using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.BLL.Interfaces;
using ISpan.InseparableCore.ViewModels;
using ISpan.InseparableCore.ViewModels.TCinemasVM;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class CinemaRepository:ICinemaRepository
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
            CTCinemasVM vm = new CTCinemasVM();
            vm.cinemas = data;
            return vm;
        }
        public void Create(CinemaEntity entity)
        {
            if (entity == null)
                throw new Exception("資料傳輸錯誤");

            try
            {
                _db.TCinemas.Add(entity.cinemas);
                _db.SaveChanges();
            }catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public TCinemas GetByName(string fCinemaName)
        {
            if (fCinemaName == null)
                throw new Exception("資料傳輸錯誤");

            var data = _db.TCinemas.FirstOrDefault(t => t.FCinemaName.Equals(fCinemaName));

            return data;
        }
        public void Edit(CinemaEntity entity)
        {
            if (entity == null)
                throw new Exception("資料傳輸錯誤!");

            var edit = GetCinema(entity.FCinemaId);

            edit.FCinemaName = entity.FCinemaName;
            edit.FCinemaAddress = entity.FCinemaAddress;
            edit.FCinemaTel = entity.FCinemaTel;
            edit.FCinemaRegion = entity.FCinemaRegion;
            edit.FLat = entity.FLat;
            edit.FLng = entity.FLng;
            edit.FTraffic = entity.FTraffic;

            try
            {
                _db.SaveChanges();
            }catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Delete(int? id)
        {
            if (id == null)
                throw new Exception("資料傳輸錯誤");

            var delete =_db.TCinemas.FirstOrDefault(t => t.FCinemaId == id);

            if(delete == null)
                throw new Exception("資料傳輸錯誤");
            try
            {
                _db.TCinemas.Remove(delete);
                _db.SaveChanges();
            }catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
