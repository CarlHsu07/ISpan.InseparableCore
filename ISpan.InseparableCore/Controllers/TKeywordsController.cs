using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models;

namespace ISpan.InseparableCore.Controllers
{
    public class TKeywordsController : Controller
    {
        private readonly InseparableContext _context;

        public TKeywordsController(InseparableContext context)
        {
            _context = context;
        }

        // GET: TKeywords
        public async Task<IActionResult> Index()
        {
              return _context.TKeywords != null ? 
                          View(await _context.TKeywords.ToListAsync()) :
                          Problem("Entity set 'InseparableContext.TKeywords'  is null.");
        }

        // GET: TKeywords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TKeywords == null)
            {
                return NotFound();
            }

            var tKeywords = await _context.TKeywords
                .FirstOrDefaultAsync(m => m.FKeywordId == id);
            if (tKeywords == null)
            {
                return NotFound();
            }

            return View(tKeywords);
        }

        // GET: TKeywords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TKeywords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FKeywordId,FKeywordName")] TKeywords tKeywords)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tKeywords);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tKeywords);
        }

        // GET: TKeywords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TKeywords == null)
            {
                return NotFound();
            }

            var tKeywords = await _context.TKeywords.FindAsync(id);
            if (tKeywords == null)
            {
                return NotFound();
            }
            return View(tKeywords);
        }

        // POST: TKeywords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FKeywordId,FKeywordName")] TKeywords tKeywords)
        {
            if (id != tKeywords.FKeywordId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tKeywords);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TKeywordsExists(tKeywords.FKeywordId))
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
            return View(tKeywords);
        }

        // GET: TKeywords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TKeywords == null)
            {
                return NotFound();
            }

            var tKeywords = await _context.TKeywords
                .FirstOrDefaultAsync(m => m.FKeywordId == id);
            if (tKeywords == null)
            {
                return NotFound();
            }

            return View(tKeywords);
        }

        // POST: TKeywords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TKeywords == null)
            {
                return Problem("Entity set 'InseparableContext.TKeywords'  is null.");
            }
            var tKeywords = await _context.TKeywords.FindAsync(id);
            if (tKeywords != null)
            {
                _context.TKeywords.Remove(tKeywords);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TKeywordsExists(int id)
        {
          return (_context.TKeywords?.Any(e => e.FKeywordId == id)).GetValueOrDefault();
        }
    }
}
