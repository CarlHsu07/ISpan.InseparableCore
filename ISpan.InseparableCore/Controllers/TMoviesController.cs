using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.ViewModels;

namespace ISpan.InseparableCore.Controllers
{
    public class TMoviesController : Controller
    {
        private readonly InseparableContext _context;

        public TMoviesController(InseparableContext context)
        {
            _context = context;
        }

        // GET: TMovies
        public async Task<IActionResult> Index()
        {
            var inseparableContext = _context.TMovies.Include(t => t.FMovieLevel);
            return View(await inseparableContext.ToListAsync());
        }

        // GET: TMovies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TMovies == null)
            {
                return NotFound();
            }

            var tMovies = await _context.TMovies
                .Include(t => t.FMovieLevel)
                .FirstOrDefaultAsync(m => m.FMovieId == id);
            if (tMovies == null)
            {
                return NotFound();
            }

            return View(tMovies);
        }

        // GET: TMovies/Create
        public IActionResult Create()
        {
            ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName");
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			return View();
        }

        // POST: TMovies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( MovieCreateVm movieCreateVm)
        {
            //if (ModelState.IsValid)
            {				
				TMovies movie = new TMovies()
				{
					FMovieIntroduction = movieCreateVm.FMovieIntroduction,
					FMovieName = movieCreateVm.FMovieName,
					FMovieLevelId = movieCreateVm.FMovieLevelId,
					FMovieOnDate = movieCreateVm.FMovieOnDate,
					FMovieOffDate = movieCreateVm.FMovieOffDate,
					FMovieLength = movieCreateVm.FMovieLength,
					FMovieImagePath = movieCreateVm.FMovieImagePath,
					FMovieScore = movieCreateVm.FMovieScore,
				};
                _context.Add(movie);
				_context.SaveChanges();

				int movieId = _context.TMovies.First(t => t.FMovieName == movieCreateVm.FMovieName).FMovieId;
				List<int> categoryIds = movieCreateVm.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i)).ToList();
				foreach (var id in categoryIds)
				{
					TMovieCategoryDetails detail = new TMovieCategoryDetails()
					{
						FMovieId = movieId,
						FMovieCategoryId = id
					};
					_context.Add(detail);
				}
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
            }
            ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", movieCreateVm.FMovieLevelId);
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			return View(movieCreateVm);
        }

        // GET: TMovies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TMovies == null)
            {
                return NotFound();
            }

            var tMovies = await _context.TMovies.FindAsync(id);
            if (tMovies == null)
            {
                return NotFound();
            }
            ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", tMovies.FMovieLevelId);
            return View(tMovies);
        }

        // POST: TMovies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FMovieId,FMovieName,FMovieIntroduction,FMovieLevelId,FMovieOnDate,FMovieOffDate,FMovieLength,FMovieImagePath,FMovieScore")] TMovies tMovies)
        {
            if (id != tMovies.FMovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tMovies);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TMoviesExists(tMovies.FMovieId))
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
            ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", tMovies.FMovieLevelId);
            return View(tMovies);
        }

        // GET: TMovies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TMovies == null)
            {
                return NotFound();
            }

            var tMovies = await _context.TMovies
                .Include(t => t.FMovieLevel)
                .FirstOrDefaultAsync(m => m.FMovieId == id);
            if (tMovies == null)
            {
                return NotFound();
            }

            return View(tMovies);
        }

        // POST: TMovies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TMovies == null)
            {
                return Problem("Entity set 'InseparableContext.TMovies'  is null.");
            }
            var tMovies = await _context.TMovies.FindAsync(id);
            if (tMovies != null)
            {
                _context.TMovies.Remove(tMovies);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TMoviesExists(int id)
        {
          return (_context.TMovies?.Any(e => e.FMovieId == id)).GetValueOrDefault();
        }
    }
}
