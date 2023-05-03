using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using X.PagedList;
using ISpan.InseparableCore.Models.DAL.Repo;
using NuGet.Protocol;
using System.Text.Json;
using System.Text.Json.Serialization;
using ISpan.InseparableCore.Models.BLL.Interface;
using ISpan.InseparableCore.Models.BLL;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class TSessionsController : Controller
    {
        private readonly InseparableContext _context;
        private readonly SessionRepository session_repo;
        private readonly MovieRepository movie_repo;
        private readonly RoomRepository room_repo;

        public TSessionsController(InseparableContext context)
        {
            _context = context;
            session_repo = new SessionRepository(context);
            movie_repo = new MovieRepository(context, null);
            room_repo = new RoomRepository(context);
        }
        
        // GET: TSessions
        public async Task<IActionResult> Index()
        {
            var inseparableContext = session_repo.SessionSearch(null);
            if (inseparableContext == null)
                return RedirectToAction("Index", "Admin");

            var pagesize = 10;
            var pageIndex = 1;

            var pagedItems = inseparableContext.Skip((pageIndex - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.page =GetPage.GetPagedProcess(pageIndex, pagesize, inseparableContext);
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaName");
            ViewData["FMovieId"] = new SelectList(movie_repo.GetByOffDay(), "FMovieId", "FMovieName");
            return View(pagedItems);
        }
        //Ajax Post
        [HttpPost]
        public async Task<IActionResult> Index(CSessionSearch vm)
        {
            var inseparableContext = session_repo.SessionSearch(vm);
            if (inseparableContext == null)
                return RedirectToAction("Index", "Admin");

            var pagesize =10;
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
        // GET: TSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TSessions == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var tSessions = session_repo.GetSession(id);
            if (tSessions == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(tSessions);
        }

        // GET: TSessions/Create
        public IActionResult Create()
        {
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaName");
            ViewData["FMovieId"] = new SelectList(movie_repo.GetByOffDay(), "FMovieId", "FMovieName");
            return View();
        }

        // POST: TSessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FSessionId,FMovieId,FRoomId,FCinemaId,FSessionDate,FSessionTime,FTicketPrice")] SessionCreateVM tSessions)
        {
            ISessionRepository repo = new SessionRepository(_context);
            SessionService service = new SessionService(repo);

            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaName", tSessions.FCinemaId);
            ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieName", tSessions.FMovieId);
            ViewData["FRoomId"] = new SelectList(_context.TRooms, "FRoomId", "FRoomName", tSessions.FRoomId);

            if (ModelState.IsValid)
            {
                try
                {
                    service.Create(tSessions.session);
                }
                catch (Exception ex)
                {
                    ViewBag.error = $"{ex.Message}";
                    return View(tSessions);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tSessions);
        }

        // GET: TSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TSessions == null)
            {
                return RedirectToAction(nameof(Index));
            }

            SessionEditVM vm = new SessionEditVM();
            var tSessions = await _context.TSessions.FindAsync(id);
            if (tSessions == null)
            {
                return RedirectToAction(nameof(Index));
            }
            vm.FSessionId = tSessions.FSessionId;
            vm.FMovieId = tSessions.FMovieId;
            vm.FRoomId = tSessions.FRoomId;
            vm.FTicketPrice = tSessions.FTicketPrice;
            vm.FCinemaId = tSessions.FCinemaId;
            vm.FSessionDate = tSessions.FSessionDate;
            vm.FSessionTime = tSessions.FSessionTime;
            
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaName", tSessions.FCinemaId);
            ViewData["FMovieId"] = new SelectList(movie_repo.GetByOffDay(), "FMovieId", "FMovieName", tSessions.FMovieId);
            ViewData["FRoomId"] = new SelectList(room_repo.GetByCinema(tSessions.FCinemaId), "FRoomId", "FRoomName", tSessions.FRoomId);
            return View(vm);
        }

        // POST: TSessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FSessionId,FMovieId,FRoomId,FCinemaId,FSessionDate,FSessionTime,FTicketPrice")] SessionEditVM tSessions)
        {
            if (id != tSessions.FSessionId)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["FCinemaId"] = new SelectList(_context.TCinemas, "FCinemaId", "FCinemaName", tSessions.FCinemaId);
            ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieName", tSessions.FMovieId);
            ViewData["FRoomId"] = new SelectList(room_repo.GetByCinema(tSessions.FCinemaId), "FRoomId", "FRoomName", tSessions.FRoomId);

            ISessionRepository repo = new SessionRepository(_context);
            SessionService service = new SessionService(repo);
            if (ModelState.IsValid)
            {
                try
                {
                    service.Edit(tSessions.session);
                }
                catch (Exception ex)
                {
                    if (!TSessionsExists(tSessions.FSessionId))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.error = $"{ex.Message}";
                        return View(tSessions);
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(tSessions);
        }

        // GET: TSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TSessions == null)
            {
                return RedirectToAction(nameof(Index)); ;
            }

            var tSessions = session_repo.GetSession(id);
            if (tSessions == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.error = TempData["error"];
            return View(tSessions);
        }

        // POST: TSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_context.TSessions == null)
            {
                return Problem("Entity set 'InseparableContext.TSessions'  is null.");
            }
            var tSessions = session_repo.GetOneSession(id);
            if (tSessions != null)
            {
                try
                {
                    session_repo.Delete(tSessions);
                }
                catch (Exception ex)
                {
                    ViewBag.error = ex.Message;
                    return RedirectToAction("Delete", new { id });
                }

            }

            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TSessionsExists(int id)
        {
            return (_context.TSessions?.Any(e => e.FSessionId == id)).GetValueOrDefault();
        }

        //Ajax
        /// <summary>
        /// 限制場次可選取時間 因每部電影播放時段不同
        /// </summary>
        /// <param name="movie">電影id</param>
        /// <returns></returns>
        public IActionResult GetDate(int? movie)
        {
            if (movie == null)
                return null;
            var max = movie_repo.GetOneMovie(movie).FMovieOffDate.Value.Date.ToString("yyyy-MM-dd");
            var min = movie_repo.GetOneMovie(movie).FMovieOnDate.Date.ToString("yyyy-MM-dd");
            return Ok(new
            {
                max=max,
                min=min,
            }.ToJson());
        }

        /// <summary>
        /// 限制可選擇影廳 每家電影院的影廳不同
        /// </summary>
        /// <param name="cinema">影院id</param>
        /// <returns></returns>
        public IActionResult GetRoom(int? cinema)
        {
            if (cinema == null)
                return null;

            var data = room_repo.GetByCinema(cinema).ToJson();

            return Ok(data);
        }
    }
}
