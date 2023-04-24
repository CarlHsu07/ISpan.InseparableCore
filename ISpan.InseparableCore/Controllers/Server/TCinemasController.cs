﻿using System;
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
        public IPagedList<CTCinemasVM> SessionPageList(int? pageIndex, int? pageSize, List<CTCinemasVM> vm)
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
            var data = cinema_repo.CinemaSearch(null);
            int pageIndex = 1;
            int pagesize = 5;
            var pagedItems = data.Skip((pageIndex - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.page = SessionPageList(pageIndex, pagesize, data);
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
                return NotFound();
            }

            var tCinemas =cinema_repo.GetCinema(id);

            if (tCinemas == null)
            {
                return NotFound();
            }

            return View(tCinemas);
        }

        // GET: TCinemas/Create
        public IActionResult Create()
        {
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
                return NotFound();
            }

            var tCinemas = await _context.TCinemas.FindAsync(id);
            if (tCinemas == null)
            {
                return NotFound();
            }
            return View(tCinemas);
        }

        // POST: TCinemas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FCinemaId,FCinemaName,FCinemaRegion,FCinemaAddress,FCinemaTel,FLat,FLng,FTraffic")] TCinemas tCinemas)
        {
            if (id != tCinemas.FCinemaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tCinemas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TCinemasExists(tCinemas.FCinemaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
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
                return NotFound();
            }

            var tCinemas = await _context.TCinemas
                .FirstOrDefaultAsync(m => m.FCinemaId == id);
            if (tCinemas == null)
            {
                return NotFound();
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
            var tCinemas = await _context.TCinemas.FindAsync(id);
            if (tCinemas != null)
            {
                _context.TCinemas.Remove(tCinemas);
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
