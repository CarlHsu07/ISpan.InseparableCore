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
    public class TCommentsController : Controller
    {
        private readonly InseparableContext _context;

        public TCommentsController(InseparableContext context)
        {
            _context = context;
        }

        // GET: TComments
        public async Task<IActionResult> Index()
        {
            var inseparableContext = _context.TComments.Include(t => t.FArticle).Include(t => t.FMember);
            return View(await inseparableContext.ToListAsync());
        }

        // GET: TComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TComments == null)
            {
                return NotFound();
            }

            var tComments = await _context.TComments
                .Include(t => t.FArticle)
                .Include(t => t.FMember)
                .FirstOrDefaultAsync(m => m.FCommentId == id);
            if (tComments == null)
            {
                return NotFound();
            }

            return View(tComments);
        }

        // GET: TComments/Create
        public IActionResult Create()
        {
            ViewData["FArticleId"] = new SelectList(_context.TArticles, "FArticleId", "FArticleContent");
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FEmail");
            return View();
        }

        // POST: TComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FCommentId,FArticleId,FItemNumber,FMemberId,FCommentPostingDate,FCommentLikes,FCommentContent")] TComments tComments)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tComments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FArticleId"] = new SelectList(_context.TArticles, "FArticleId", "FArticleContent", tComments.FArticleId);
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FEmail", tComments.FMemberId);
            return View(tComments);
        }

        // GET: TComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TComments == null)
            {
                return NotFound();
            }

            var tComments = await _context.TComments.FindAsync(id);
            if (tComments == null)
            {
                return NotFound();
            }
            ViewData["FArticleId"] = new SelectList(_context.TArticles, "FArticleId", "FArticleContent", tComments.FArticleId);
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FEmail", tComments.FMemberId);
            return View(tComments);
        }

        // POST: TComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FCommentId,FArticleId,FItemNumber,FMemberId,FCommentPostingDate,FCommentLikes,FCommentContent")] TComments tComments)
        {
            if (id != tComments.FCommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tComments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TCommentsExists(tComments.FCommentId))
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
            ViewData["FArticleId"] = new SelectList(_context.TArticles, "FArticleId", "FArticleContent", tComments.FArticleId);
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FEmail", tComments.FMemberId);
            return View(tComments);
        }

        // GET: TComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TComments == null)
            {
                return NotFound();
            }

            var tComments = await _context.TComments
                .Include(t => t.FArticle)
                .Include(t => t.FMember)
                .FirstOrDefaultAsync(m => m.FCommentId == id);
            if (tComments == null)
            {
                return NotFound();
            }

            return View(tComments);
        }

        // POST: TComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TComments == null)
            {
                return Problem("Entity set 'InseparableContext.TComments'  is null.");
            }
            var tComments = await _context.TComments.FindAsync(id);
            if (tComments != null)
            {
                _context.TComments.Remove(tComments);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TCommentsExists(int id)
        {
          return (_context.TComments?.Any(e => e.FCommentId == id)).GetValueOrDefault();
        }
    }
}
