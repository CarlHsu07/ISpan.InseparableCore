using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using System.Runtime.Intrinsics.X86;
using X.PagedList;
using Azure;
using System.Drawing.Printing;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class TSessionsController : Controller
    {
        private readonly InseparableContext _context;

        public TSessionsController(InseparableContext context)
        {
            _context = context;
        }
        //pagelist
        /// <summary>
        /// 分頁
        /// </summary>
        /// <param name="page">第幾頁</param>
        /// <param name="pageSize">一頁幾個</param>
        /// <param name="vm">資料</param>
        /// <returns></returns>
       public IPagedList<CSessionVM> SessionPageList(int? pageIndex, int? pageSize, List<CSessionVM> vm)
        {
            if (!pageIndex.HasValue || pageIndex < 1)
                return null;
            IPagedList<CSessionVM> pagelist = vm.ToPagedList(pageIndex??1, (int)pageSize);
            if (pagelist.PageNumber != 1 && pageIndex.HasValue && pageIndex > pagelist.PageCount)
                return null;
            return pagelist;
        }
        // GET: TSessions
        public async Task<IActionResult> Index()
        {
            List<CSessionVM> data =new List<CSessionVM>();
            var inseparableContext = _context.TSessions.OrderByDescending(t=>t.FSessionDate).Include(t => t.FCinema).Include(t => t.FMovie).Include(t => t.FRoom);
            foreach(var item in inseparableContext)
            {
                CSessionVM vm = new CSessionVM();
                vm.session = item;
                vm.FMovie=item.FMovie;
                vm.FCinema = item.FCinema;
                vm.FRoom = item.FRoom;

                data.Add(vm);
            }
            var pagesize = 5;
            var pageIndex = 1;
            var count =data.Count();
            var totalpage = (int)Math.Ceiling(count / (double)pagesize);  //無條件進位
            var pagedItems = data.Skip((pageIndex - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.page = SessionPageList(pageIndex, pagesize, data);
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaName");
            ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieName");
            return View(pagedItems);
        }

        // GET: TSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TSessions == null)
            {
                return NotFound();
            }

            var tSessions = await _context.TSessions
                .Include(t => t.FCinema)
                .Include(t => t.FMovie)
                .Include(t => t.FRoom)
                .FirstOrDefaultAsync(m => m.FSessionId == id);
            if (tSessions == null)
            {
                return NotFound();
            }

            return View(tSessions);
        }

        // GET: TSessions/Create
        public IActionResult Create()
        {
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaName");
            ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieName");
            ViewData["FRoomId"] = new SelectList(_context.TRooms, "FRoomId", "FRoomName");
            return View();
        }

        // POST: TSessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FSessionId,FMovieId,FRoomId,FCinemaId,FSessionDate,FSessionTime,FTicketPrice")] TSessions tSessions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tSessions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaAddress", tSessions.FCinemaId);
            ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieImagePath", tSessions.FMovieId);
            ViewData["FRoomId"] = new SelectList(_context.TRooms, "FRoomId", "FRoomName", tSessions.FRoomId);
            return View(tSessions);
        }

        // GET: TSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TSessions == null)
            {
                return NotFound();
            }

            var tSessions = await _context.TSessions.FindAsync(id);
            if (tSessions == null)
            {
                return NotFound();
            }
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaAddress", tSessions.FCinemaId);
            ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieImagePath", tSessions.FMovieId);
            ViewData["FRoomId"] = new SelectList(_context.TRooms, "FRoomId", "FRoomName", tSessions.FRoomId);
            return View(tSessions);
        }

        // POST: TSessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FSessionId,FMovieId,FRoomId,FCinemaId,FSessionDate,FSessionTime,FTicketPrice")] TSessions tSessions)
        {
            if (id != tSessions.FSessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tSessions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TSessionsExists(tSessions.FSessionId))
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
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaAddress", tSessions.FCinemaId);
            ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieImagePath", tSessions.FMovieId);
            ViewData["FRoomId"] = new SelectList(_context.TRooms, "FRoomId", "FRoomName", tSessions.FRoomId);
            return View(tSessions);
        }

        // GET: TSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TSessions == null)
            {
                return NotFound();
            }

            var tSessions = await _context.TSessions
                .Include(t => t.FCinema)
                .Include(t => t.FMovie)
                .Include(t => t.FRoom)
                .FirstOrDefaultAsync(m => m.FSessionId == id);
            if (tSessions == null)
            {
                return NotFound();
            }

            return View(tSessions);
        }

        // POST: TSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TSessions == null)
            {
                return Problem("Entity set 'InseparableContext.TSessions'  is null.");
            }
            var tSessions = await _context.TSessions.FindAsync(id);
            if (tSessions != null)
            {
                _context.TSessions.Remove(tSessions);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TSessionsExists(int id)
        {
          return (_context.TSessions?.Any(e => e.FSessionId == id)).GetValueOrDefault();
        }
    }
}
