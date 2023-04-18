using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using NuGet.Protocol;

namespace ISpan.InseparableCore.Controllers
{
    public class TArticlesController : Controller
    {
        private readonly InseparableContext _context;
		private readonly ArticleRepository repo;

		public TArticlesController(InseparableContext context)
        {
            _context = context;
			repo = new ArticleRepository(context);
		}

		// GET: TArticles
		public async Task<IActionResult> Index()
        {
			List<ArticleVm> articles = repo.Search(null).ToList();

			int pageContent = 2;
			int pageNumber = articles.Count % pageContent == 0 ? articles.Count / pageContent
														   : articles.Count / pageContent + 1;
			List<SelectListItem> pageSelectList = new List<SelectListItem>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new SelectListItem(i.ToString(), i.ToString()));
			}
			articles = articles.Take(pageContent).ToList();
			ViewData["Page"] = new SelectList(pageSelectList, "Value", "Text");

			TMovieCategories defaultCategory = new TMovieCategories() { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", 0);

			return View( articles.ToList());
        }
		[HttpPost]
		public async Task<IActionResult> Index(ArticleSearchCondition condition)
        {
			List<ArticleVm> articles = repo.Search(condition).ToList();

			int pageContent = 2;
			int pageNumber = articles.Count % pageContent == 0 ? articles.Count / pageContent
														   : articles.Count / pageContent + 1;
			List<SelectListItem> pageSelectList = new List<SelectListItem>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new SelectListItem(i.ToString(), i.ToString()));
			}
			articles = articles.Skip(pageContent * (condition.Page - 1)).Take(pageContent).ToList();
			ViewData["Page"] = new SelectList(pageSelectList, "Value", "Text");

			TMovieCategories defaultCategory = new TMovieCategories() { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", condition.CategoryId);

            return Ok( articles.ToJson());
        }

        // GET: TArticles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TArticles == null)
            {
                return NotFound();
            }

            var article = await _context.TArticles.FirstOrDefaultAsync(m => m.FArticleId == id);
			//點閱數+1
			article.FArticleClicks++;
			_context.Update(article);
			_context.SaveChanges();

			if (article == null)
            {
                return NotFound();
            }
			var vm = repo.GetVmById(article.FArticleId);

			//點閱數+1
			//repo.Click(vm);
			ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName");

			return View(vm);
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
        public async Task<IActionResult> Create(ArticleVm vm)
        {
            if (ModelState.IsValid)
            {
				repo.CreateAsync(vm);
                return RedirectToAction(nameof(Index));
            }
            ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", vm.FArticleCategoryId);
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName", vm.FMemberId);
            return View(vm);
        }

        // GET: TArticles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TArticles == null)
            {
                return NotFound();
            }

            var article = await _context.TArticles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
			var vm = article.ModelToVm();
            ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", vm.FArticleCategoryId);
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName", vm.FMemberId);
            return View(vm);
        }

        // POST: TArticles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArticleVm vm)
        {
            //if (id != vm.FArticleId)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
					repo.UpdateAsync(vm);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TArticlesExists(vm.FArticleId))
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
            ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", vm.FArticleCategoryId);
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName", vm.FMemberId);
            return View(vm);
        }

        // GET: TArticles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TArticles == null)
            {
                return NotFound();
            }

            var article = await _context.TArticles
                .Include(t => t.FArticleCategory)
                .Include(t => t.FMember)
                .FirstOrDefaultAsync(m => m.FArticleId == id);
            if (article == null)
            {
                return NotFound();
            }
			var vm = article.ModelToVm();
            return View(vm);
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
            var article = await _context.TArticles.FindAsync(id);

            if (article != null)
            {
                _context.TArticles.Remove(article);
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
