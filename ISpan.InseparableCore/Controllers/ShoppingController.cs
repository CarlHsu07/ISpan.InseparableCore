using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ISpan.InseparableCore.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly InseparableContext _db;
        public ShoppingController(InseparableContext db)
        {
            _db = db;
        }
        public IActionResult Ticket(CticketVM vm)
        {
            vm.cinema = _db.TCinemas.Select(t => t);
            vm.cinemaId = vm.cinemaId == null ? 0 : vm.cinemaId;

            //時間區間
            var start = DateTime.Now.Date;
            var end = DateTime.Now.Date.AddDays(7);

            if (vm.cinemaId != 0 )
            {
                vm.movie = _db.TSessions.Where(t => t.FCinemaId == vm.cinemaId).Select(t => t.FMovie).Distinct();
                vm.movieId = vm.movieId == null ? 0 : vm.movieId;
            }
            if (vm.movieId != 0)
            {
                var date = _db.TSessions.Where(t => t.FCinemaId == vm.cinemaId && t.FMovieId == vm.movieId).GroupBy(t => t.FSessionDate).Select(t => t.Key);
                vm.sessions = new Dictionary<DateTime, IEnumerable<TSessions>>();
                foreach (var item in date)
                {
                    var sessions = _db.TSessions.Where(t => t.FCinemaId == vm.cinemaId && t.FMovieId == vm.movieId && t.FSessionDate == item); // &&t.FSessionDate>=start && t.FSessionDate<=end

                    vm.sessions.Add(item, sessions);
                }
            }
            return View(vm);
        }

        public IActionResult Booking(int? cinema, int? session)
        {
            CbookingVM vm = new CbookingVM();
            if (session == null)
            {
                return RedirectToAction("Ticket");
            }
            vm.sessions = _db.TSessions.Where(t => t.FSessionId == session);
            vm.movie = _db.TSessions.Where(t => t.FSessionId == session).Select(t => t.FMovie);
            vm.cinema = _db.TSessions.Where(t => t.FSessionId == session).Select(t => t.FCinema);
            vm.products = _db.TProducts.Where(t => t.FCinemaId == cinema);
            return View(vm);
        }

        public IActionResult CartItem(int? productId, int? quantity)
        {
            string responseText = "fail";
            var item = _db.TProducts.FirstOrDefault(t=>t.FProductId==productId);

            List<CShoppingCartItem> cart = null;
            string json = string.Empty;
            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json);
            }
            else
                cart = new List<CShoppingCartItem>();

            CShoppingCartItem vm = new CShoppingCartItem();
            //todo 紀錄
            //vm.Price = (decimal)p.FPrice;
            //vm.ProductId = vm.txtfId;
            //item.Count = vm.txtCount;
            //item.Product = p;

            //cart.Add(item);
            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDitionary.SK_PURCHASED_PRODUCTS_LIST, json);



            //test
            if (productId!=null && quantity!=null)
                responseText = "pass";

            return Ok(responseText);
        }
    }
}
