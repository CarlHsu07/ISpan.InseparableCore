using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.Models.DAL.Repo;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Protocol;

namespace ISpan.InseparableCore.Controllers
{
    public class CinemaController : Controller
    {
        private readonly InseparableContext _db;
        private readonly ApiKeys _key;
        private readonly CinemaRepository _repo;
        public CinemaController(InseparableContext db, IOptions<ApiKeys> key)
        {
            _db = db;
            _key = key.Value;
            _repo = new CinemaRepository(db);
        }

        public IActionResult Cinema()
        {
            CcinemaVM vm = new CcinemaVM();
            //分區 //todorepo
            vm.city = _db.TCities.Select(t => t.FCityName).ToList();

            //分品牌
            vm.brand = new List<string> { "威秀", "秀泰", "國賓" };

            return View(vm);
        }


        //電影院Ajax傳輸
        public IActionResult City(string name)
        {
            var data = _repo.GetByCity(name).ToJson();
            return Ok(data);
        }
        public IActionResult Brand(string name)
        {
            var data = _repo.GetByBrand(name).ToJson();
            return Ok(data);
        }
        public IActionResult Map(int? id)
        {
            CMapVM vm = new CMapVM();
            var data = _db.TCinemas.FirstOrDefault(t => t.FCinemaId == id);

            if (data == null)
                return BadRequest("糟糕...出現錯誤");

            vm.Name = data.FCinemaName;
            vm.FLat = data.FLat;
            vm.FLng = data.FLng;
            vm.FTraffic = data.FTraffic.Split("<br>").ToList();
            vm.Key = _key.MapKey;
            return Ok(vm.ToJson());
        }
    }
}
