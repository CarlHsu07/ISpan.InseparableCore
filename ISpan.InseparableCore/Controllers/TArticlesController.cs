﻿using System;
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
    public class TArticlesController : Controller
    {
        private readonly InseparableContext _context;

        public TArticlesController(InseparableContext context)
        {
            _context = context;
        }

		// GET: TArticles
        public async Task<IActionResult> Index()
        {
            var inseparableContext = _context.TArticles.Include(t => t.FArticleCategory).Include(t => t.FMember);

			ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName");

			return View(await inseparableContext.ToListAsync());
        }
		[HttpPost]
        public IActionResult Index(ArticleSearchCondition condition)
        {
            IEnumerable<TArticles> inseparableContext = _context.TArticles.Include(t => t.FArticleCategory).Include(t => t.FMember);

			if (condition.ArticleId.HasValue) inseparableContext = inseparableContext.Where(t => t.FArticleId == condition.ArticleId);
			if (!string.IsNullOrEmpty(condition.Title)) inseparableContext = inseparableContext.Where(t => t.FArticleTitle.Contains(condition.Title));
			if (condition.CategoryId.HasValue) inseparableContext = inseparableContext.Where(t => t.FArticleCategoryId == condition.CategoryId);
			if (condition.MemberId.HasValue) inseparableContext = inseparableContext.Where(t => t.FMemberId == condition.MemberId);

			ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName");

			return View(inseparableContext.ToList());
        }

        // GET: TArticles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TArticles == null)
            {
                return NotFound();
            }

            var tArticles = await _context.TArticles
                .Include(t => t.FArticleCategory)
                .Include(t => t.FMember)
                .FirstOrDefaultAsync(m => m.FArticleId == id);
            if (tArticles == null)
            {
                return NotFound();
            }

            return View(tArticles);
        }

        // GET: TArticles/Create
        public IActionResult Create()
        {
            ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName");
            return View();
        }

        // POST: TArticles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FArticleId,FArticleTitle,FMemberId,FArticleCategoryId,FArticlePostingDate,FArticleLikes,FArticleClicks,FArticleContent")] TArticles tArticles)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tArticles);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", tArticles.FArticleCategoryId);
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName", tArticles.FMemberId);
            return View(tArticles);
        }

        // GET: TArticles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TArticles == null)
            {
                return NotFound();
            }

            var tArticles = await _context.TArticles.FindAsync(id);
            if (tArticles == null)
            {
                return NotFound();
            }
            ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", tArticles.FArticleCategoryId);
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName", tArticles.FMemberId);
            return View(tArticles);
        }

        // POST: TArticles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FArticleId,FArticleTitle,FMemberId,FArticleCategoryId,FArticlePostingDate,FArticleLikes,FArticleClicks,FArticleContent")] TArticles tArticles)
        {
            if (id != tArticles.FArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tArticles);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TArticlesExists(tArticles.FArticleId))
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
            ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", tArticles.FArticleCategoryId);
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName", tArticles.FMemberId);
            return View(tArticles);
        }

        // GET: TArticles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TArticles == null)
            {
                return NotFound();
            }

            var tArticles = await _context.TArticles
                .Include(t => t.FArticleCategory)
                .Include(t => t.FMember)
                .FirstOrDefaultAsync(m => m.FArticleId == id);
            if (tArticles == null)
            {
                return NotFound();
            }

            return View(tArticles);
        }

        // POST: TArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TArticles == null)
            {
                return Problem("Entity set 'InseparableContext.TArticles'  is null.");
            }
            var tArticles = await _context.TArticles.FindAsync(id);
            if (tArticles != null)
            {
                _context.TArticles.Remove(tArticles);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TArticlesExists(int id)
        {
          return (_context.TArticles?.Any(e => e.FArticleId == id)).GetValueOrDefault();
        }
    }
}
