using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace ISpan.InseparableCore.Controllers
{
    public class CinemaController : Controller
    {
        private InseparableContext _db;
        public CinemaController(InseparableContext db)
        {
            _db = db;
        }

        public IActionResult Cinema()
        {
            CcinemaVM vm = new CcinemaVM();
            //分區
            vm.city = _db.TCities.Select(t => t.FCityName).ToList();


            //分品牌
            vm.brand = new List<string> { "威秀", "秀泰", "國賓" };

            return View(vm);
        }


        //電影院Ajax傳輸
        public IActionResult City(string name)
        {
            var data = _db.TCinemas.Where(t => t.FCinemaRegion == name).ToJson();
            return Ok(data);
        }
        public IActionResult Brand(string name)
        {
            var data = _db.TCinemas.Where(t => t.FCinemaName.Contains(name)).ToJson();
            return Ok(data);
        }
        public IActionResult Map(int? id)
        {
            CMapVM vm = new CMapVM();
            var data = _db.TCinemas.FirstOrDefault(t => t.FCinemaId == id);

            vm.Name = data.FCinemaName;
            vm.FLat = data.FLat;
            vm.FLng = data.FLng;
            vm.FTraffic = data.FTraffic.Split("<br>").ToList();
            vm.Key = "ZPyRU3c5rVSNfr62GfzHZ5HzQnb01eaHY6z11OZ_Ke0";
            return Ok(vm.ToJson());
        }
    }
}
