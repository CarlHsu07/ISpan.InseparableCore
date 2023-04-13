﻿using ISpan.InseparableCore.Models;
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


        //Ajax傳輸
        public IActionResult City(string city)
        {
            var data = _db.TCinemas.Where(t => t.FCinemaRegion == city).ToJson();
            return Ok(data);
        }
        public IActionResult Brand(string brand)
        {
            var data = _db.TCinemas.Where(t => t.FCinemaName.Contains(brand)).ToJson();
            return Ok(data);
        }
        public IActionResult Map(int? id)
        {
            CMapVM vm = new CMapVM();
            var data = _db.TCinemas.FirstOrDefault(t => t.FCinemaId == id);


            vm.FLat = data.FLat;
            vm.FLng = data.FLng;
            vm.FTraffic = data.FTraffic.Split("<br>").ToList();
            vm.Key = "ZPyRU3c5rVSNfr62GfzHZ5HzQnb01eaHY6z11OZ_Ke0";
            return Ok(vm.ToJson());
        }
    }
}
