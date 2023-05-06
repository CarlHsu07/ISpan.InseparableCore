using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class AdminMovieCategoriesController : AdminSuperController
    {
        private readonly InseparableContext _context;

        public AdminMovieCategoriesController(InseparableContext context)
        {
            _context = context;
        }

        // GET: AdminMovieCategories
        public async Task<IActionResult> Index()
        {
              return _context.TMovieCategories != null ? 
                          View(await _context.TMovieCategories.ToListAsync()) :
                          Problem("Entity set 'InseparableContext.TMovieCategories'  is null.");
        }


        // POST: AdminMovieCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( TMovieCategories tMovieCategories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tMovieCategories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tMovieCategories);
        }

        // POST: AdminMovieCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TMovieCategories tMovieCategories)
        {
            if (id != tMovieCategories.FMovieCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tMovieCategories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TMovieCategoriesExists(tMovieCategories.FMovieCategoryId))
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
            return View(tMovieCategories);
        }


        // POST: AdminMovieCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.TMovieCategories == null)
            {
                return Problem("Entity set 'InseparableContext.TMovieCategories'  is null.");
            }
            var tMovieCategories = await _context.TMovieCategories.FindAsync(id);
            if (tMovieCategories != null)
            {
                _context.TMovieCategories.Remove(tMovieCategories);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TMovieCategoriesExists(int id)
        {
          return (_context.TMovieCategories?.Any(e => e.FMovieCategoryId == id)).GetValueOrDefault();
        }
    }
}
