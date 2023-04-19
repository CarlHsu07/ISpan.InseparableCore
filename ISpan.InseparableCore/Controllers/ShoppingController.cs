using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.BLL.Interfaces;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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
        private readonly ApiKeys _key;
        private readonly OrderRepository _order_repo;
        private readonly TicketOrderRepository _ticket_repo;
        private readonly ProductOrderRepository _product_repo;
        private readonly SessionRepository _session_repo;
        private readonly CinemaRepository _cinema_repo;
        public ShoppingController(InseparableContext db,IOptions<ApiKeys> key)
        {
            _db = db;
            _key = key.Value;
            _order_repo = new OrderRepository(db);
            _ticket_repo = new TicketOrderRepository(db);
            _product_repo = new ProductOrderRepository(db);
            _session_repo = new SessionRepository(db);
            _cinema_repo = new CinemaRepository(db);
        }
        //todo 防止上一頁錯誤
        public IActionResult Ticket(CticketVM vm)
        {
            //以防萬一只要一開啟訂購畫面 第一件事清空session
            HttpContext.Session.Remove(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
            HttpContext.Session.Remove(CDitionary.SK_PURCHASED_TICKET_LIST);

            vm.cinema = _cinema_repo.QueryAll();
            vm.cinemaId = vm.cinemaId == null ? 0 : vm.cinemaId;

            //限制時間區間
            var start = DateTime.Now.Date;
            var now = DateTime.Now.TimeOfDay;
            var end = DateTime.Now.Date.AddDays(7);
            //todo時間限制還沒放
            if (vm.cinemaId != 0)
            {
                vm.movie = _session_repo.GetMovieByCinema(vm.cinemaId);
                // &&t.FSessionDate>=start && t.FSessionDate<=end 
                vm.movieId = vm.movieId == null ? 0 : vm.movieId;
            }

            if (vm.movieId != 0)
            {
                var date = _session_repo.GetSession(vm.cinemaId,vm.movieId).GroupBy(t => t.FSessionDate).Select(t => t.Key);
                // &&t.FSessionDate>=start && t.FSessionDate<=end 
                vm.sessions = new Dictionary<DateTime, IEnumerable<TSessions>>();
                foreach (var item in date)
                {
                    var sessions = _session_repo.GetSession(vm.cinemaId, vm.movieId).Where(t=>t.FSessionDate == item);
                    //&&t.FsessionTime>=now

                    vm.sessions.Add(item, sessions);
                }
            }
            return View(vm);
        }

        public IActionResult Booking(int? cinema, int? session)
        {
            CbookingVM vm = new CbookingVM();
            if (session == null ||cinema==null)
            {
                return RedirectToAction("Ticket");
            }
            vm.sessions = _session_repo.GetSessionBySession(session);
            vm.movie = _session_repo.GetMovieBySEssion(session);
            vm.cinema = _session_repo.GetCinemaBySEssion(session);
            vm.products = _db.TProducts.Where(t => t.FCinemaId == cinema);  //todo productrepo

            return View(vm);
        }

        //Ajax
        public IActionResult CartItem(int? productId, int? quantity)
        {
            //產品紀錄在session
            string responseText = "fail";

            if (productId == null)
                return Ok(responseText);
            var product = _db.TProducts.FirstOrDefault(t => t.FProductId == productId); //todo productrepo

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
            if (vm == null || vm.sessionid == null)
            {
                string error = "網頁加載時出現問題";
                return RedirectToAction("Error", new { error });
            }
            vm.solid = new List<int>();

            var solid = _ticket_repo.GetSolid(vm.sessionid ,true);
            foreach (var item in solid)
            {
                vm.solid.Add(item.FSeatId);
            }

            vm.seats = new Dictionary<string, IEnumerable<TSeats>>();
            var row = _db.TSeats.GroupBy(t => t.FSeatRow).Select(t => t.Key); //todo seatrepo
            foreach (var item in row)
            {
                var column = _db.TSeats.Where(t => t.FSeatRow == item); //todo seatrepo
                vm.seats.Add(item, column);
            }

            vm.sessions = _session_repo.GetOneSession(vm.sessionid);
            vm.movie = _session_repo.GetMovieBySEssion(vm.sessionid);
            return View(vm);
        }

        //Ajax
        public IActionResult TicketItem(int? seatId, int? Qty, int? sessionId)
        {
            //將票券座位記錄在session
            string responseText = "fail";

            if (seatId == null || sessionId == null)
                return Ok(responseText);

            var session = _session_repo.GetOneSession(sessionId);

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
                item.FMovieName = _db.TMovies.FirstOrDefault(t => t.FMovieId == session.FMovieId).FMovieName; //todo movierepo
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
        //todo cache err_cache_miss
        public IActionResult CartView(int? regular, int? concession, int? sessionid)
        {
            if (regular == null || concession == null || sessionid == null)
            {
                string error = "網頁加載時出現問題";
                return RedirectToAction("Error", new { error });
            }
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

                var seat = _db.TSeats.Where(t => t.FSeatId == item); //todo seatrepo
                foreach (var name in seat)
                {
                    seatid = name.FSeatRow + name.FSeatColumn;
                    vm.seats.Add(item, seatid);
                }
            }
            vm.concession = (int)concession;
            vm.regular = (int)regular;
            vm.session = _session_repo.GetOneSession(sessionid);
            vm.movies = _session_repo.GetMovieBySEssion(sessionid);
            vm.cart = cart;

            return View(vm);
        }

        //todo 如何知道下單的是誰
        public IActionResult CashPay(CorderVM vm)
        {
            var orderid = DbSave(vm);
            if (orderid == null)
            {
                string error = "位置已售出請重新選擇!";
                return RedirectToAction("Error", new { error });
            }
            var order = _order_repo.GetById(orderid);
            order.FStatus = true;

            var ticket =_ticket_repo.GetById(orderid);
            foreach (var item in ticket)
            {
                item.FStatus = true;
            }

            _db.SaveChanges();
            HttpContext.Session.Remove(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
            HttpContext.Session.Remove(CDitionary.SK_PURCHASED_TICKET_LIST);
            return View(order);
        }
        //綠界API
        public IActionResult CreditPay(CorderVM vm)
        {
            var orderid = DbSave(vm);
            if (orderid == null)
            {
                string error = "位置已售出請重新選擇!";
                return RedirectToAction("Error", new { error });
            }
            //綠界
            var TradeNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            var web = "https://localhost:7021/"; //todo 上線要改
            //var web = _envior.WebRootPath;
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
                { "ClientBackURL",$"{web}Shopping/Ticket"},
                { "ChoosePayment","Credit"},
                { "EncryptType","1"},
            };
            order["CheckMacValue"] = GetCheckMacValue(order);

            var get = _order_repo.GetById(orderid);
            if (get == null)
            {
                string error = "訂單出現錯誤!請重新下單";
                return RedirectToAction("Error", new { error });
            }
            get.FCreditTradeNo = TradeNo;
            _db.SaveChanges();

            return View(order);

        }
        [HttpPost]
        public HttpResponseMessage AddPayInfo(JObject info)
        {
            //todo 不確定這裡要做什麼判斷
            if (!HttpContext.Session.Keys.Contains(CDitionary.SK_PURCHASED_TICKET_LIST))
            {
                return ResponseOK();
            }
            else
            {
                return ResponseError();
            }
        }

        [HttpPost]
        public IActionResult Paydone(int? id)
        {
            if (id == null)
            {
                string error = "網頁加載時出現問題";
                return RedirectToAction("Error", new { error });
            }
            var order = _order_repo.GetById(id);
            if (order == null)
            {
                string error = "網頁加載時出現問題";
                return RedirectToAction("Error", new { error });
            }

            order.FStatus = true;

            var ticket = _ticket_repo.GetById(order.FOrderId);
            foreach (var item in ticket)
            {
                item.FStatus = true;
            }

            _db.SaveChanges();
            HttpContext.Session.Remove(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
            HttpContext.Session.Remove(CDitionary.SK_PURCHASED_TICKET_LIST);
            return View(order);
        }

        //錯誤view
        public IActionResult Error(string error)
        {
            ViewBag.error = error;
            return View();
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

            try
            {
                _order_repo.Create(vm.orders);
            }
            catch (Exception ex)
            {
                return null;
            }

            int orderid = _order_repo.GetByAll(vm.orders).FOrderId;

            //ticket
            foreach (var item in ticket_list)
            {
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
                ITicketOrderRepository repo = new TicketOrderRepository(_db);
                TicketOrderService service = new TicketOrderService(repo);
                try
                {
                    service.Create(item.ticket);
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    HttpContext.Session.Remove(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
                    HttpContext.Session.Remove(CDitionary.SK_PURCHASED_TICKET_LIST);
                    return null;
                }

            }

            //product
            if(product_list != null)
            {
                foreach (var item in product_list)
                {
                    item.FOrderId = orderid;
                    item.FProductDiscount = 1;
                    item.FProductSubtotal = item.FProductUnitprice * item.FProductDiscount;

                    try
                    {
                        _product_repo.Create(item.ProductOrderDetails);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
            }
           
            HttpContext.Session.Remove(CDitionary.SK_PURCHASED_PRODUCTS_LIST);
            HttpContext.Session.Remove(CDitionary.SK_PURCHASED_TICKET_LIST);
            return orderid;
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
            var hashKey = _key.hashKey;

            //測試用的 HashIV
            var HashIV = _key.HashIV;

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
        //回應
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
