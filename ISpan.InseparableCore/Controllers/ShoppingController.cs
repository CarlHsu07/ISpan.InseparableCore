using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.BLL.Interfaces;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.Models.DAL.Repo;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using prjMvcCoreDemo.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace ISpan.InseparableCore.Controllers
{
    public class ShoppingController :  SuperController
    {
        private readonly InseparableContext _db;
        private readonly ApiKeys _key;
        private readonly OrderRepository _order_repo;
        private readonly TicketOrderRepository _ticket_repo;
        private readonly ProductOrderRepository _product_order_repo;
        private readonly SessionRepository _session_repo;
        private readonly CinemaRepository _cinema_repo;
        private readonly ProductRepository _product_repo;
        private readonly SeatRepository _seat_repo;
        private readonly MovieRepository _movie_repo;
        public ShoppingController(InseparableContext db,IOptions<ApiKeys> key)
        {
            _db = db;
            _key = key.Value;
            _order_repo = new OrderRepository(db);
            _ticket_repo = new TicketOrderRepository(db);
            _product_order_repo = new ProductOrderRepository(db);
            _session_repo = new SessionRepository(db);
            _cinema_repo = new CinemaRepository(db);
            _product_repo = new ProductRepository(db);
            _seat_repo = new SeatRepository(db);
            _movie_repo = new MovieRepository(db, null);
        }
        public IActionResult Ticket(CticketVM vm)
        {
            //以防萬一只要一開啟訂購畫面 第一件事清空訂單session
            HttpContext.Session.Remove(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            HttpContext.Session.Remove(CDictionary.SK_PURCHASED_TICKET_LIST);

            vm.cinema = _cinema_repo.QueryAll();
            if(vm.cinema==null)
                return RedirectToAction("Index", "Home");

            vm.cinemaId = vm.cinemaId == null ? 0 : vm.cinemaId;

            //限制時間區間
            var now = DateTime.Now.TimeOfDay;
            var start = DateTime.Now.Date;

            if (vm.cinemaId != 0)
            {
                vm.movie = _session_repo.GetMovieByCinema(vm.cinemaId);
                if (vm.movie.Count()==0)
                    return View(vm);
                vm.movieId = vm.movieId == null ? 0 : vm.movieId;
            }

            if (vm.movieId > 0)
            {
                var date = _session_repo.GetSessionByTwoCondition(vm.cinemaId,vm.movieId).OrderBy(t=>t.FSessionDate).GroupBy(t => t.FSessionDate).Select(t => t.Key);
                if (date.Count() == 0)
                    return View(vm);
                vm.sessions = new Dictionary<DateTime, IEnumerable<TSessions>>();
                foreach (var item in date)
                {
                    IEnumerable<TSessions> sessions = null;

                    sessions = _session_repo.GetSessionByTwoCondition(vm.cinemaId, vm.movieId).Where(t => t.FSessionDate == item).OrderBy(t=>t.FSessionTime);
                    if (item==start)
                        sessions = _session_repo.GetSessionByTwoCondition(vm.cinemaId, vm.movieId).Where(t => t.FSessionDate == item && t.FSessionTime>now).OrderBy(t => t.FSessionTime);

                    if(sessions.Count()!=0)
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
            vm.products = _product_repo.GetProductByCinema(cinema);

            return View(vm);
        }

        //Ajax
        public IActionResult ProductItem(int? productId, int? quantity)
        {
            //產品紀錄在session
            string responseText = "fail";

            if (productId == null)
                return Ok(responseText);
            var product = _product_repo.GetOneProduct(productId);

            if (product == null)
                return BadRequest("糟糕...出錯了");

            List<CproductCartItem> cart = null;
            string json = string.Empty;
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
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
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);
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
            var row = _seat_repo.GetSeat().GroupBy(t => t.FSeatRow).Select(t => t.Key);
            if( row.Count()==0)
            {
                string error = "網頁加載時出現問題";
                return RedirectToAction("Error", new { error });
            }
            foreach (var item in row)
            {
                var column = _seat_repo.GetSeat().Where(t => t.FSeatRow == item);
                if(column.Count()!=0)
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
            if(session ==null)
            {
                return BadRequest("糟糕...出錯啦");
            }
            List<CticketCartItemVM> cart = null;
            string json = string.Empty;
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_TICKET_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_TICKET_LIST);
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
                item.FMovieName = _movie_repo.GetOneMovie(session.FMovieId).FMovieName;
                item.FRoomId = session.FRoomId;
                item.FSeatId = (int)seatId;
                item.FSessionId = (int)sessionId;

                cart.Add(item);
            }

            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_TICKET_LIST, json);
            responseText = "pass";
            return Ok(responseText);
        }
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

            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CproductCartItem>>(json);
            }

            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_TICKET_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_TICKET_LIST);
                ticket = JsonSerializer.Deserialize<List<CticketCartItemVM>>(json);
            }
            if (ticket == null)
                return RedirectToAction("seat");

            vm.seats = new Dictionary<int, string>();
            var seats = ticket.Select(t => t.FSeatId); 
            if (seats == null)
            {
                string error = "網頁加載時出現問題";
                return RedirectToAction("Error", new { error });
            }
            foreach (var item in seats)
            {
                var seat = _seat_repo.GetSeat().Where(t => t.FSeatId == item);
                if (seat.Count()==0)
                {
                    string error = "網頁加載時出現問題";
                    return RedirectToAction("Error", new { error });
                }
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

        public IActionResult CashPay(CorderForDbVM vm)
        {
            if(vm==null)
            {
                string error = "網頁加載時出現問題 請重新下單!!";
                return RedirectToAction("Error", new { error });
            }
            var orderid = DbSave(vm);

            if (orderid == null)
            {
                string error = "位置已售出請重新選擇!";
                return RedirectToAction("Error", new { error });
            }
            var order = _order_repo.GetById(orderid);
            if (order == null)
            {
                string error = "網頁加載時出現問題 請重新下單!";
                return RedirectToAction("Error", new { error });
            }
            order.FStatus = true;

            var ticket =_ticket_repo.GetById(orderid);
            if (ticket.Count()==0)
            {
                string error = "網頁加載時出現問題 請重新下單!!";
                return RedirectToAction("Error", new { error });
            }
            foreach (var item in ticket)
            {
                item.FStatus = true;
            }

            _db.SaveChanges();

            try
            {
                SendEmail(orderid);

            }
            catch (Exception)
            {
                string error = "Oops";
                return RedirectToAction("Error", new { error });
            }
            
            HttpContext.Session.Remove(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            HttpContext.Session.Remove(CDictionary.SK_PURCHASED_TICKET_LIST);
            HttpContext.Session.Remove(CDictionary.SK_ORDER_ID);

            return View(order);
        }
        //綠界API
        public IActionResult CreditPay(CorderForDbVM vm)
        {
            var orderid = DbSave(vm);
            if (orderid == null)
            {
                string error = "位置已售出請重新選擇!";
                return RedirectToAction("Error", new { error });
            }
            string json = JsonSerializer.Serialize(orderid);
            HttpContext.Session.SetString(CDictionary.SK_ORDER_ID,json);
            //綠界
            var TradeNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            var web = "inseparable.fun"; //todo 上線要改
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
                { "OrderResultURL",$"{web}Shopping/Paydone/?id={orderid}&tradeNo={TradeNo}"},
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

            ViewBag.order = orderid;
            return View(order);

        }
        [HttpPost]
        public HttpResponseMessage AddPayInfo(JObject info)
        {
            //todo 不確定這裡要做什麼判斷 等上線測試
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_TICKET_LIST))
            {
                return ResponseOK();
            }
            else
            {
                return ResponseError();
            }
            //todo 好像因為沒有網址所以不會跑
        }
        //[HttpPost]
        //public IActionResult AddPayInfo(ECPayResponse info)
        //{
        //    //todo 不確定這裡要做什麼判斷
        //    string error = string.Empty;
        //    string json = string.Empty;
        //    int? id = null;
        //    if (HttpContext.Session.Keys.Contains(CDictionary.SK_ORDER_ID))
        //    {
        //        json = HttpContext.Session.GetString(CDictionary.SK_ORDER_ID);
        //        id = JsonSerializer.Deserialize<int>(json);
        //    }
        //    if (info.RtnCode != 1)
        //    {
        //        var ticket = _ticket_repo.GetById(id);
        //        if (ticket == null)
        //        {
        //            return Content("0|Error");
        //        }

        //        foreach (var item in ticket)
        //        {
        //            item.FStatus = false;
        //        }

        //        _db.SaveChanges();
        //    }
        //    var order = _order_repo.GetOneOrder(id);
        //    if (order == null)
        //    {
        //        return Content("0|Error");
        //    }
        //    if (info.MerchantTradeNo != order.FCreditTradeNo)
        //    {
        //        return Content("0|Error");
        //    }
        //    return Content("1|OK");
        //}
        [HttpPost]
        public IActionResult Paydone(int? id,string tradeNo)
        {
            if (id == null)
            {
                string error = "網頁加載時出現問題";
                return RedirectToAction("Error", new { error });
            }
            var order = _order_repo.GetById(id);
            if (order == null)
            {
                string error = "網頁加載時出現問題 請重新下單!";
                return RedirectToAction("Error", new { error });
            }
            order.FStatus = true;

            _db.SaveChanges();

            try
            {
                SendEmail(id);

            }
            catch (Exception)
            {
                string error = "Oops";
                return RedirectToAction("Error", new { error });
            }

            HttpContext.Session.Remove(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            HttpContext.Session.Remove(CDictionary.SK_PURCHASED_TICKET_LIST);
            HttpContext.Session.Remove(CDictionary.SK_ORDER_ID);

            ViewBag.No = tradeNo;
            return View();
        }

        //錯誤view
        public IActionResult Error(string error)
        {
            ViewBag.error = error;
            return View();
        }
        //db儲存
        public int? DbSave(CorderForDbVM vm)
        {
            List<CproductCartItem> product_list = null;
            List<CticketCartItemVM> ticket_list = null;
            string json = string.Empty;

            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                product_list = JsonSerializer.Deserialize<List<CproductCartItem>>(json);
            }

            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_TICKET_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_TICKET_LIST);
                ticket_list = JsonSerializer.Deserialize<List<CticketCartItemVM>>(json);
            }
            if (ticket_list == null)
                return null;
            //order 
            vm.FOrderDate = DateTime.Now;
            vm.FModifiedTime = DateTime.Now;
            vm.FMemberId = _user.FId==0? 1:_user.FId; //todo 要不要讓訪客下單
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
                item.Fstatus = true;  //todo 問題為當使用者再付款畫面關閉 如何將座位釋出 目前解決辦法為在後台寫一隻code去撈問題資料
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
                    HttpContext.Session.Remove(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                    HttpContext.Session.Remove(CDictionary.SK_PURCHASED_TICKET_LIST);
                    return null;
                }
                
            }

            //product
            if (product_list != null)
            {
                foreach (var item in product_list)
                {
                    item.FOrderId = orderid;
                    item.FProductDiscount = 1;
                    item.FProductSubtotal = item.FProductUnitprice * item.FProductDiscount;

                    try
                    {
                        _product_order_repo.Create(item.ProductOrderDetails);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
            }
           
            return orderid;
        }

        public IActionResult Order()
        {
            string error = string.Empty;
            string json = string.Empty;
            int? id = null;
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_ORDER_ID))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_ORDER_ID);
                id = JsonSerializer.Deserialize<int>(json);
            }
            if (id == null)
            {
                error = "網頁加載時出現問題";
                return RedirectToAction("Error", new { error });
            }
            var ticket = _ticket_repo.GetById(id);
            if (ticket.Count()==0)
            {
                error = "網頁加載時出現問題";
                return RedirectToAction("Error", new { error });
            }

            foreach(var item in ticket)
            {
                item.FStatus = false;
            }

            _db.SaveChanges();

            error = "結帳失敗!!";
            return RedirectToAction("Error", new { error });
        }
        //清除session
        public IActionResult Clearticket()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_TICKET_LIST))
                HttpContext.Session.Remove(CDictionary.SK_PURCHASED_TICKET_LIST);

            return Ok();
        }
        public IActionResult Clearproduct()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
                HttpContext.Session.Remove(CDictionary.SK_PURCHASED_PRODUCTS_LIST);

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


        //Email寄送
        public void SendEmail(int? id)
        {
            if (id == null)
                throw new Exception("找不到訂單");

            var ticket = _ticket_repo.GetById(id);
            if (ticket.Count() == 0)
                throw new Exception("找不到訂單");

            if (_user.FEmail == null)
            {
                var member = _db.TOrders.FirstOrDefault(t => t.FOrderId == id).FMemberId;
                _user = _db.TMembers.FirstOrDefault(t => t.FId == member);
            }

            var product = _product_order_repo.GetById(id);
            string body = $"<h3>{_user.FFirstName}您好，您有新訂單:</h3><h5>INSEPARABLE感謝您的訂購!</h5>\r\n<div style=\"text-align:center;\">\r\n<div style=\"border:1.5px #E7B152 solid; text-align:left; padding:5px;\">\r\n<p>訂購日期:${DateTime.Now.ToString("yyyy/MM/dd  HH:mm")}</p><p>訂購商品如下:</p><table style=\"border:solid \t#AD5A5A 1.5px;border-radius:5px; width:100%;\" border=\"1\">\r\n<thead style=\"background-color:black;color:white;\">\r\n<tr>\r\n <th>項次</th>\r\n<th>電影</th>\r\n<th>場次</th>\r\n<th>座位</th>\r\n<th>票價</th>\r\n</tr>\r\n</thead>\r\n<tbody style=\"background-color:#181616;color: white;\">\r\n";
            int count = 0;
            foreach(var item in ticket)
            {
                count += 1;
                body += $"<tr>\r\n<td>{count}</td>\r\n<td>{item.FMovieName}</td>\r\n<td>{item.FSession.FSessionDate.ToString("yyyy/MM/dd")}         {item.FSession.FSessionTime.Hours} : {item.FSession.FSessionTime.Minutes.ToString("D2")}</td>\r\n<td>{item.FSeat.FSeatRow}{item.FSeat.FSeatColumn}</td>\r\n<td>{item.FTicketUnitprice.ToString("###")}</td>\r\n</tr>";
            }
            body += "\r\n</tbody>\r\n</table>\r\n";
            if (product.Count()!=0)
            {
                count = 0;
                body += $"<br /><table style=\"border:solid \t#AD5A5A 1.5px;border-radius:5px; width:100%;\" border=\"1\">\r\n<thead style=\"background-color:black;color:white;\">\r\n<tr>\r\n<th>項次</th>\r\n<th>商品</th>\r\n<th>數量</th>\r\n<th>單價</th>\r\n<th>小記</th>\r\n</tr>\r\n</thead>\r\n<tbody style=\"background-color:#181616;color: white;\">";
                foreach (var item in product)
                {
                    count += 1;
                    body += $"\r\n<tr>\r\n<td>{count}</td>\r\n<td>{item.FProductName}</td>\r\n<td>{item.FProductQty}</td>\r\n<td>{item.FProductUnitprice.ToString("###")}</td>\r\n<td>{item.FProductSubtotal.ToString("###")}</td>\r\n</tr>\r\n";
                }
                body += "\r\n</tbody>\r\n</table>";
            }
            body += $"<br /></div><br /></div><a href=\"#\"><p style=\"color:\t#FF0000\">INSEPARABLE</p></a>";//todo 網址
            SmtpClient mysmpt = new SmtpClient("smtp-mail.outlook.com", 587);
            mysmpt.Credentials = new NetworkCredential(_key.Email, _key.Password);
            mysmpt.EnableSsl = true;

            MailMessage mail = new MailMessage();
            mail.To.Add(_user.FEmail);  //todo 刷卡後偵測不到
            mail.From = new MailAddress(_key.Email, "INSEPARABLE", System.Text.Encoding.UTF8);
            mail.Priority = MailPriority.Normal;
            mail.Subject = "[訂單]您在INSEPARABLE,訂單資料";
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = body;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mysmpt.Send(mail);
            
        }
    }
}
