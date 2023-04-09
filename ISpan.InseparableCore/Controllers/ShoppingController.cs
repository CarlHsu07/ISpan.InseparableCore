using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
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

            if (productId == null)
                return Ok(responseText);
            var product = _db.TProducts.FirstOrDefault(t=>t.FProductId==productId);

            List<CproductCartItem> cart = null;
            string json = string.Empty;
            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CproductCartItem>>(json);
            }
            else
                cart = new List<CproductCartItem>();

            CproductCartItem delete = null;
            if(cart != null)
            {
                foreach(var p in cart)
                {
                    if(p.FProductId== productId)
                    {
                        delete=cart.FirstOrDefault(c=>c.FProductId==p.FProductId);
                    }
                }
                cart.Remove(delete);
            }
            if (quantity > 0)
            {
            CproductCartItem item = new CproductCartItem();
            item.FProductItemNo = cart.Count()>0? cart.Count()+1:1;
            item.FProductUnitprice = product.FProductUnitprice;
            item.FProductQty = (int)quantity;
            item.FProduct = product;
            item.FProductId = (int)productId;
            item.FProductName = product.FProductName;

            cart.Add(item); 
            }
            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDitionary.SK_PURCHASED_PRODUCTS_LIST, json);
            if(cart.Count()>0)
                responseText = "pass";
            if (quantity == 0)
                responseText = "fail";

            return Ok(responseText);
        }
        public IActionResult Seat(CseatVM vm)
        {
            if (vm.sessionid == null)
                return View();
            vm.solid = new List<int>();

            var solid = _db.TTicketOrderDetails.Where(t => t.FSessionId == vm.sessionid);
            foreach (var item in solid)
            {
                vm.solid.Add(item.FSeatId);
            }

            vm.seats = new Dictionary<string, IEnumerable<TSeats>>();
            var row = _db.TSeats.GroupBy(t => t.FSeatRow).Select(t => t.Key);
            foreach (var item in row)
            {
                var column = _db.TSeats.Where(t => t.FSeatRow == item);
                vm.seats.Add(item, column);
            }

            vm.sessions = _db.TSessions.FirstOrDefault(t => t.FSessionId == vm.sessionid);
            vm.movie = _db.TSessions.Where(t => t.FSessionId == vm.sessionid).Select(t => t.FMovie);
            return View(vm);
        }
        public IActionResult TicketItem(int? seatId,int? Qty, int? sessionId)
        {
            string responseText = "fail";

            if (seatId == null)
                return Ok(responseText);

            var session = _db.TSessions.FirstOrDefault(t => t.FSessionId == sessionId);

            List<CticketCartItemVM> cart = null;
            string json = string.Empty;
            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_TICKET_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_TICKET_LIST);
                cart = JsonSerializer.Deserialize<List<CticketCartItemVM>>(json);
            }
            else
                cart = new List<CticketCartItemVM>();

            CticketCartItemVM delete = null;
            if (Qty == 0)
            {
                delete=cart.FirstOrDefault(c=>c.FSeatId== seatId);
                cart.Remove(delete);
            }
            else
            {
                CticketCartItemVM item =new CticketCartItemVM();
                item.FTicketUnitprice = (decimal)session.FTicketPrice;
                item.FTicketItemNo = cart.Count() > 0 ? cart.Count() + 1 : 0;
                item.FMovieId = session.FMovieId;
                item.FRoomId = session.FRoomId;
                item.FSeatId = (int)seatId;
                item.FSessionId = (int)sessionId;

                cart.Add(item);
            }

            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDitionary.SK_PURCHASED_TICKET_LIST, json);

            return Ok(responseText);
        }
        public IActionResult CartView(int? regular,int? concession,int? sessionid)
        {
            CcartviewVM vm =new CcartviewVM();
            string json = null;
            List<CproductCartItem> cart = null;
            List<CticketCartItemVM> ticket=null;
            string seatid = string.Empty;
      
            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CproductCartItem>>(json);
            }

            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_TICKET_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_TICKET_LIST);
                ticket = JsonSerializer.Deserialize<List<CticketCartItemVM>>(json);
            }
            vm.seats = new Dictionary<int, string>();
            var seats = ticket.Select(t => t.FSeatId);
            foreach(var item in seats)
            {
                var seat = _db.TSeats.Where(t => t.FSeatId == item);
                foreach(var name in seat)
                {
                    seatid = name.FSeatRow + name.FSeatColumn;
                    vm.seats.Add(item,seatid);
                }
            }
            vm.concession = (int)concession;
            vm.regular = (int)regular;
            vm.session = _db.TSessions.FirstOrDefault(t => t.FSessionId == sessionid);
            vm.movies = _db.TSessions.Where(t => t.FSessionId == sessionid).Select(t=>t.FMovie);
            vm.cart = cart;
            
            return View(vm);
        }
        //如果直截用網頁返回session要如何清除重來
        //location.href是否可以用post
        //問題!!要怎麼知道這一筆detial是哪個orderid???
        public IActionResult Pay(int? regular,int? cocession)
        {
            List<CproductCartItem> cart = null;
            List<CticketCartItemVM> ticket = null;
            string json = string.Empty;

            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CproductCartItem>>(json);
            }

            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_TICKET_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_TICKET_LIST);
                ticket = JsonSerializer.Deserialize<List<CticketCartItemVM>>(json);
            }


            return View();
        }
    }
}
