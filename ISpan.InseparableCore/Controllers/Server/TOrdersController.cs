﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using ISpan.InseparableCore.Models.DAL.Repo;
using X.PagedList;
using NuGet.Protocol;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class TOrdersController : Controller
    {
        private readonly InseparableContext _context;
        private readonly OrderRepository order_repo;
        private readonly TicketOrderRepository ticket_repo;
        private readonly ProductOrderRepository product_repo;

        public TOrdersController(InseparableContext context)
        {
            _context = context;
            order_repo = new OrderRepository(context);
            ticket_repo=new TicketOrderRepository(context);
            product_repo=new ProductOrderRepository(context);
        }
        public IPagedList<CorderVM> SessionPageList(int? pageIndex, int? pageSize, List<CorderVM> vm)
        {
            if (!pageIndex.HasValue || pageIndex < 1)
                return null;
            IPagedList<CorderVM> pagelist = vm.ToPagedList(pageIndex ?? 1, (int)pageSize);
            if (pagelist.PageNumber != 1 && pageIndex.HasValue && pageIndex > pagelist.PageCount)
                return null;
            return pagelist;
        }
        // GET: TOrders
        public async Task<IActionResult> Index()
        {
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaName");
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FMemberId");

            var inseparableContext = order_repo.GetOrder(null);
            var pagesize = 5;
            var pageIndex = 1;

            var pagedItems = inseparableContext.Skip((pageIndex - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.page = SessionPageList(pageIndex, pagesize, inseparableContext);

            return View(pagedItems);
        }
        [HttpPost]
        public async Task<IActionResult> Index(CorderSearch vm)
        {

            var inseparableContext = order_repo.GetOrder(vm);
            List<TMovies> movie = new List<TMovies>();
            List<TCinemas> cinema = new List<TCinemas>();
            List<TSessions> session = new List<TSessions>();
            var pagesize = 5;
            var pageIndex = vm.pageIndex;
            var count = inseparableContext.Count();
            var totalpage = (int)Math.Ceiling(count / (double)pagesize);  //無條件進位
            var pagedItems = inseparableContext.Skip((pageIndex - 1) * pagesize).Take(pagesize).ToList();
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            string json = JsonSerializer.Serialize(pagedItems, options);
            return Ok(new
            {
                Items = json,
                totalpage = totalpage,
            }.ToJson());

        }
        // GET: TOrders/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.TOrders == null)
            {
                return NotFound();
            }
            CorderDetaillVM vm =new CorderDetaillVM();

            vm.orders = order_repo.GetOneOrder(id);
            vm.ticket=ticket_repo.GetById(id);
            vm.product = product_repo.GetById(id);
            vm.FCinema = vm.orders.FCinema;
            vm.FMember = vm.orders.FMember;
            if (vm.orders== null)
            {
                return NotFound();
            }
            
            return View(vm);
        }       

        // GET: TOrders/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || _context.TOrders == null)
            {
                return NotFound();
            }
            CorderVM vm =new CorderVM();
            vm.orders = order_repo.GetOneOrder(id);
            vm.FCinema=vm.orders.FCinema;
            vm.FMember=vm.orders.FMember;
            if (vm.orders == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // POST: TOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TOrders == null)
            {
                return Problem("Entity set 'InseparableContext.TOrders'  is null.");
            }
            try
            {
               order_repo.Delete(id);

            }catch(Exception ex)
            {
                ViewBag.error = ex.Message;
                return RedirectToAction("Delete", new { id });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}