using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

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

            //限制時間區間
            var start = DateTime.Now.Date;
            var starttime = DateTime.Now.TimeOfDay;
            var end = DateTime.Now.Date.AddDays(7);
            //todo時間限制還沒放
            if (vm.cinemaId != 0)
            {
                vm.movie = _db.TSessions.Where(t => t.FCinemaId == vm.cinemaId).Select(t => t.FMovie).Distinct();
                // &&t.FSessionDate>=start && t.FSessionDate<=end 
                vm.movieId = vm.movieId == null ? 0 : vm.movieId;
            }

            if (vm.movieId != 0)
            {
                var date = _db.TSessions.Where(t => t.FCinemaId == vm.cinemaId && t.FMovieId == vm.movieId).GroupBy(t => t.FSessionDate).Select(t => t.Key);
                // &&t.FSessionDate>=start && t.FSessionDate<=end 
                vm.sessions = new Dictionary<DateTime, IEnumerable<TSessions>>();
                foreach (var item in date)
                {
                    var sessions = _db.TSessions.Where(t => t.FCinemaId == vm.cinemaId && t.FMovieId == vm.movieId && t.FSessionDate == item);
                    //&&t.FsessionTime>=starttime

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

        //Ajax
        public IActionResult CartItem(int? productId, int? quantity)
        {
            //產品紀錄在session
            string responseText = "fail";

            if (productId == null)
                return Ok(responseText);
            var product = _db.TProducts.FirstOrDefault(t => t.FProductId == productId);

            List<CproductCartItem> cart = null;
            string json = string.Empty;
            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CproductCartItem>>(json);
            }
            else
                cart = new List<CproductCartItem>();

            //同一商品只記錄一次
            CproductCartItem delete = null;
            foreach (var p in cart)
            {
                if (p.FProductId == productId)
                {
                    delete = cart.FirstOrDefault(c => c.FProductId == p.FProductId);
                }
            }
            cart.Remove(delete);

            if (quantity > 0)
            {
                CproductCartItem item = new CproductCartItem();
                item.FProductItemNo = cart.Count() > 0 ? cart.Count() + 1 : 1;
                item.FProductUnitprice = product.FProductUnitprice;
                item.FProductQty = (int)quantity;
                item.FProductId = (int)productId;
                item.FProductName = product.FProductName;

                cart.Add(item);
            }
            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDitionary.SK_PURCHASED_PRODUCTS_LIST, json);
            if (cart.Count() > 0)
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

            var solid = _db.TTicketOrderDetails.Where(t => t.FSessionId == vm.sessionid && t.FStatus == true);
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

        //Ajax
        public IActionResult TicketItem(int? seatId, int? Qty, int? sessionId)
        {
            //將票券座位記錄在session
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

            //位置取消session也要刪除
            CticketCartItemVM delete = null;
            if (Qty == 0)
            {
                delete = cart.FirstOrDefault(c => c.FSeatId == seatId);
                cart.Remove(delete);
            }
            else
            {
                CticketCartItemVM item = new CticketCartItemVM();
                item.FTicketUnitprice = (decimal)session.FTicketPrice;
                item.FTicketItemNo = cart.Count() > 0 ? cart.Count() + 1 : 1;
                item.FMovieId = session.FMovieId;
                item.FMovieName = _db.TMovies.FirstOrDefault(t => t.FMovieId == session.FMovieId).FMovieName;
                item.FRoomId = session.FRoomId;
                item.FSeatId = (int)seatId;
                item.FSessionId = (int)sessionId;

                cart.Add(item);
            }

            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDitionary.SK_PURCHASED_TICKET_LIST, json);
            responseText = "pass";
            return Ok(responseText);
        }
        public IActionResult CartView(int? regular, int? concession, int? sessionid)
        {
            CcartviewVM vm = new CcartviewVM();
            string json = null;
            List<CproductCartItem> cart = null;
            List<CticketCartItemVM> ticket = null;
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
            foreach (var item in seats)
            {

                var seat = _db.TSeats.Where(t => t.FSeatId == item);
                foreach (var name in seat)
                {
                    seatid = name.FSeatRow + name.FSeatColumn;
                    vm.seats.Add(item, seatid);
                }
            }
            vm.concession = (int)concession;
            vm.regular = (int)regular;
            vm.session = _db.TSessions.FirstOrDefault(t => t.FSessionId == sessionid);
            vm.movies = _db.TSessions.Where(t => t.FSessionId == sessionid).Select(t => t.FMovie);
            vm.cart = cart;

            return View(vm);
        }

        //todo 如何知道下單的是誰
        //todo DB跟綠界順序
        public IActionResult CashPay(CorderVM vm)
        {
            var orderid = DbSave(vm);
            if (orderid == null)
            {
                ViewBag.error = "資料有誤！！";
                return View();
            }
            var order = _db.TOrders.FirstOrDefault(t => t.FOrderId == orderid);
            order.FStatus = true;

            var ticket = _db.TTicketOrderDetails.Where(t => t.FOrderId == orderid);
            foreach (var item in ticket)
            {
                item.FStatus = true;
            }

            _db.SaveChanges();
            HttpContext.Session.Clear();
            return View(order);
        }
        //綠界API
        public IActionResult CreditPay(CorderVM vm)
        {
            var orderid =DbSave(vm);
            if (orderid == null)
            {
                ViewBag.errpr = "位置已售出請重新選擇!";
                return View();
            }
            //綠界
            var TradeNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            var web = "https://localhost:7021/"; //todo 上線要改
            var order = new Dictionary<string, string>
            {
                { "MerchantID",  "3002607"},
                { "MerchantTradeNo",TradeNo},
                { "MerchantTradeDate",DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
                { "PaymentType","aio"},
                { "TotalAmount",$"{vm.FTotalMoney}"},
                { "TradeDesc","無"},
                { "ItemName","電影票"},
                { "ReturnURL",$"{web}Shopping/AddPayInfo"},
                { "OrderResultURL",$"{web}Shopping/Paydone/{orderid}"},
                { "ChoosePayment","Credit"},
                { "EncryptType","1"},
            };
            order["CheckMacValue"] = GetCheckMacValue(order);
            return View(order);

        }
        [HttpPost]
        public HttpResponseMessage AddPayInfo(JObject info)
        {

            if (!HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_TICKET_LIST))
            {
                return ResponseOK();
            }
            else
            {
                return ResponseError();
            }       
        }
        public IActionResult Paydone(int? id)
        {
            if (id == null)
            {
                ViewBag.error = "資料有誤！！";
                return View();
            }
            var order = _db.TOrders.FirstOrDefault(t => t.FOrderId == id);
            order.FStatus = true;

            var ticket = _db.TTicketOrderDetails.Where(t => t.FOrderId == id);
            foreach(var item in ticket)
            {
                item.FStatus = true;
            }

            _db.SaveChanges();
            HttpContext.Session.Clear();
            return View(order);
        }
        //db儲存
        public int? DbSave(CorderVM vm)
        {
            List<CproductCartItem> product_list = null;
            List<CticketCartItemVM> ticket_list = null;
            string json = string.Empty;

            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
                product_list = JsonSerializer.Deserialize<List<CproductCartItem>>(json);
            }

            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_TICKET_LIST))
            {
                json = HttpContext.Session.GetString(CDitionary.SK_PURCHASED_TICKET_LIST);
                ticket_list = JsonSerializer.Deserialize<List<CticketCartItemVM>>(json);
            }


            //order 
            vm.FOrderDate = DateTime.Now;
            vm.FModifiedTime = DateTime.Now;
            vm.FMemberId = 1; //todo 目前尚未解決登入
            vm.FStatus = false;


            _db.TOrders.Add(vm.orders);
            _db.SaveChanges();

            int orderid = _db.TOrders.FirstOrDefault(t => t == vm.orders).FOrderId;

            //ticket
            foreach (var item in ticket_list)
            {
                //驗證位置是否未售出
                var solid = _db.TTicketOrderDetails.Where(t => t.FSessionId == item.FSessionId && t.FStatus == true).FirstOrDefault(t => t.FSeatId == item.FSeatId);
                if (solid != null && solid.FOrderId != orderid)
                {
                    HttpContext.Session.Clear();
                    //throw new Exception("位置已售出請重新選擇!");
                    return null;
                }

                item.FOrderId = orderid;
                item.Fstatus = false;
                if (vm.regular > 0)
                {
                    item.FTicketDiscount = 1;
                    vm.regular -= 1;
                    item.FTicketSubtotal = item.FTicketDiscount * item.FTicketUnitprice;
                }
                else
                {
                    item.FTicketDiscount = (decimal)0.9;
                    vm.concession -= 1;
                    item.FTicketSubtotal = item.FTicketDiscount * item.FTicketUnitprice;
                }

                _db.TTicketOrderDetails.Add(item.ticket);
                _db.SaveChanges();
            }

            //product
            foreach (var item in product_list)
            {
                item.FOrderId = orderid;
                item.FProductDiscount = 1;
                item.FProductSubtotal = item.FProductUnitprice * item.FProductDiscount;
                _db.TProductOrderDetails.Add(item.ProductOrderDetails);
                _db.SaveChanges();
            }
            return orderid;
            HttpContext.Session.Clear();
        }

        //清除session
        public IActionResult Clearticket()
        {
            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_TICKET_LIST))
                HttpContext.Session.Remove(CDitionary.SK_PURCHASED_TICKET_LIST);
            return Ok();
        }
        public IActionResult Clearproduct()
        {
            if (HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_PRODUCTS_LIST))
                HttpContext.Session.Remove(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
            return Ok();
        }

        //綠界雜湊
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();

            var checkValue = string.Join("&", param);

            //測試用的 HashKey
            var hashKey = "pwFHCqoQZGmho4w6";

            //測試用的 HashIV
            var HashIV = "EkRm7iFT261dpevs";

            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";

            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();

            checkValue = GetSHA256(checkValue);

            return checkValue.ToUpper();
        }
        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            return result.ToString();
        }
        private HttpResponseMessage ResponseError()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("0|Error");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        private HttpResponseMessage ResponseOK()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("1|OK");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}
