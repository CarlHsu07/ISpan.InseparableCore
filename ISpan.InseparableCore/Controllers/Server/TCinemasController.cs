using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using X.PagedList;
using ISpan.InseparableCore.ViewModels.TCinemasVM;
using ISpan.InseparableCore.Models.DAL.Repo;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Security.Cryptography.Xml;
using System.Drawing.Printing;
using NuGet.Protocol;
using System.Text.Json.Serialization;
using System.Text.Json;
using ISpan.InseparableCore.Models.BLL.Interfaces;
using ISpan.InseparableCore.Models.BLL;
using Microsoft.AspNetCore.Http;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class TCinemasController : Controller
    {
        private readonly InseparableContext _context;
        private readonly CinemaRepository cinema_repo;
        public TCinemasController(InseparableContext context)
        {
            _context = context;
            cinema_repo = new CinemaRepository(context);
        }
        public IPagedList<CTCinemasVM> CinemaPageList(int? pageIndex, int? pageSize, List<CTCinemasVM> vm)
        {
            if (!pageIndex.HasValue || pageIndex < 1)
                return null;
            IPagedList<CTCinemasVM> pagelist = vm.ToPagedList(pageIndex ?? 1, (int)pageSize);
            if (pagelist.PageNumber != 1 && pageIndex.HasValue && pageIndex > pagelist.PageCount)
                return null;
            return pagelist;
        }
        // GET: TCinemas
        public IActionResult Index()
        {
            List<CTCinemasVM> data =cinema_repo.CinemaSearch(null);
            if (data == null)
                return RedirectToAction("Index", "Admin");

            int pageIndex = 1;
            int pagesize = 5;
            var pagedItems = data.Skip((pageIndex - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.page = CinemaPageList(pageIndex, pagesize, data);
            ViewData["FCity"] = new SelectList(_context.TCities, "FCityName", "FCityName");
            List<string> brnad = new List<string>{"威秀","秀泰","國賓",};
            SelectList brnadList = new SelectList(brnad);
            ViewBag.brand = brnadList;
            return View(pagedItems);
        }
        [HttpPost]
        public IActionResult Index(CTCinemaSearch vm)
        {
            var inseparableContext = cinema_repo.CinemaSearch(vm);
            if (inseparableContext == null)
                return RedirectToAction("Index", "Admin");

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
        // GET: TCinemas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TCinemas == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var tCinemas =cinema_repo.GetCinema(id);

            if (tCinemas == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(tCinemas);
        }

        // GET: TCinemas/Create
        public IActionResult Create()
        {
            ViewData["FCity"] = new SelectList(_context.TCities, "FCityName", "FCityName");
            return View();
        }

        // POST: TCinemas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FCinemaId,FCinemaName,FCinemaRegion,FCinemaAddress,FCinemaTel,FLat,FLng,FTraffic")] CTCinemasCreateVM tCinemas)
        {
            ICinemaRepository repo = new CinemaRepository(_context);
            CinemaService service = new CinemaService(repo);
            if (ModelState.IsValid)
            {
                try 
                { 
                service.Create(tCinemas.cinemas);
                }
                catch(Exception ex)
                {
                    ViewBag.error = $"{ex.Message}";
                    return View(tCinemas);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tCinemas);
        }

        // GET: TCinemas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TCinemas == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var tCinemas = cinema_repo.GetCinema(id);
            if (tCinemas == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["FCity"] = new SelectList(_context.TCities, "FCityName", "FCityName");
            CTCinemasCreateVM vm = new CTCinemasCreateVM();
            vm.FCinemaId = tCinemas.FCinemaId;
            vm.FCinemaName = tCinemas.FCinemaName;
            vm.FCinemaAddress = tCinemas.FCinemaAddress;
            vm.FCinemaTel = tCinemas.FCinemaTel;
            vm.FLat = tCinemas.FLat;
            vm.FLng=tCinemas.FLng;
            vm.FTraffic=tCinemas.FTraffic;
            vm.FCinemaRegion = tCinemas.FCinemaRegion;

            return View(vm);
        }

        // POST: TCinemas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FCinemaId,FCinemaName,FCinemaRegion,FCinemaAddress,FCinemaTel,FLat,FLng,FTraffic")] CTCinemasCreateVM tCinemas)
        {
            ICinemaRepository repo = new CinemaRepository(_context);
            CinemaService service = new CinemaService(repo);
            if (id != tCinemas.FCinemaId)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    service.Edit(tCinemas.cinemas);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (!TCinemasExists(tCinemas.FCinemaId))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.error = $"{ex.Message}";
                        return View(tCinemas);
                    }
                }
                return RedirectToAction(nameof(Index)); 
            } 
            return View(tCinemas);
        }

        // GET: TCinemas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TCinemas == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var tCinemas = cinema_repo.GetCinema(id);
            if (tCinemas == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(tCinemas);
        }

        // POST: TCinemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TCinemas == null)
            {
                return Problem("Entity set 'InseparableContext.TCinemas'  is null.");
            }

            try
            {
                cinema_repo.Delete(id);
            }
            catch(Exception ex)
            {
                ViewBag.error = $"{ex.Message}";
                return RedirectToAction("Delete", new { id });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TCinemasExists(int id)
        {
            return (_context.TCinemas?.Any(e => e.FCinemaId == id)).GetValueOrDefault();
        }
    }
}
