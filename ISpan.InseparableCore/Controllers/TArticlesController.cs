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
using X.PagedList;
using Azure;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using prjMvcCoreDemo.Models;
using System.Text.Json;
using System.Diagnostics.Metrics;

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

		public int GetUserId()
		{
			int userId = 0;
			if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
			{
				var serializedTMembers = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
				userId = JsonSerializer.Deserialize<TMembers>(serializedTMembers).FId;
			}
			return userId;
		}

		//產生頁碼
		protected IPagedList<ArticleVm> GetPagedProcess(int? page, int pageSize, List<ArticleVm> articles)
		{
			// 過濾從client傳送過來有問題頁數
			if (page.HasValue && page < 1)
				return null;
			// 從資料庫取得資料
			var listUnpaged = articles;
			IPagedList<ArticleVm> pagelist = listUnpaged.ToPagedList(page ?? 1, pageSize);
			// 過濾從client傳送過來有問題頁數，包含判斷有問題的頁數邏輯
			if (pagelist.PageNumber != 1 && page.HasValue && page > pagelist.PageCount)
				return null;
			return pagelist;
		}
		// GET: TArticles
		public async Task<IActionResult> Index()
		{
			List<ArticleVm> articles = repo.Search(null).ToList();

			#region ViewData

			int pageContent = 2;
			int pageNumber = articles.Count % pageContent == 0 ? articles.Count / pageContent
														   : articles.Count / pageContent + 1;
			List<SelectListItem> pageSelectList = new List<SelectListItem>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new SelectListItem(i.ToString(), i.ToString()));
			}
			//articles = articles.Take(pageContent).ToList();
			ViewData["Page"] = new SelectList(pageSelectList, "Value", "Text");

			TMovieCategories defaultCategory = new TMovieCategories() { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", 0);
			#endregion

			int pageSize = 10;

			ViewBag.ArticleModel = GetPagedProcess(1, pageSize, articles);
			articles = articles.Take(pageSize).ToList();

			return View(articles);
		}
		[HttpPost]
		public async Task<IActionResult> Index(ArticleSearchCondition condition)
		{
			List<ArticleVm> articles = repo.Search(condition).ToList();

			#region ViewData
			int pageContent = 2;
			int pageNumber = articles.Count % pageContent == 0 ? articles.Count / pageContent
														   : articles.Count / pageContent + 1;
			List<SelectListItem> pageSelectList = new List<SelectListItem>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new SelectListItem(i.ToString(), i.ToString()));
			}
			ViewData["Page"] = new SelectList(pageSelectList, "Value", "Text");

			TMovieCategories defaultCategory = new TMovieCategories() { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", condition.CategoryId);
			#endregion

			int pageSize = 10;
			var pageList = GetPagedProcess(condition.Page, pageSize, articles);
			articles = articles.Skip(pageSize * ((int)condition.Page - 1)).Take(pageSize).ToList();
			if (articles.Count == 0) return Ok("noData");

			return Ok(new
			{
				Vm = articles,
				PageCount = pageList.PageCount,
				TotalItemCount = pageList.TotalItemCount,
				PageSize = pageSize
			}.ToJson());
		}

		// GET: TArticles/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.TArticles == null)
			{
				return NotFound();
			}

			var article = await _context.TArticles.FirstOrDefaultAsync(m => m.FArticleId == id);

			if (article == null)
			{
				return NotFound();
			}
			var vm = repo.GetVmById(article.FArticleId);

			//是否點讚
			int memberId = 1;
			var detailInDb = await _context.TArticleLikeDetails.FirstOrDefaultAsync(t => t.FMemberId == memberId
																		&& t.FArticleId == vm.FArticleId);
			if (detailInDb == null) vm.LikeOrUnlike = false;
			else vm.LikeOrUnlike = true;

			//點閱數+1
			repo.Click(vm.FArticleId);
			ViewData["FMemberId"] =  new SelectList(_context.TMembers, "FId", "FFirstName");

			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> ArticleLike(TArticleLikeDetails detail)
		{
			bool like = false;
			var detailInDb = _context.TArticleLikeDetails
				.FirstOrDefault(t => t.FMemberId == detail.FMemberId
								  && t.FArticleId == detail.FArticleId);

			ArticleVm article = repo.GetVmById(detail.FArticleId);

			if (detailInDb == null)
			{
				article.FArticleLikes++;
				await repo.UpdateLikeAsync(article);

				like = true;
				_context.Add(detail);
			}
			else
			{
				article.FArticleLikes--;
				await repo.UpdateLikeAsync(article);

				like = false;
				_context.Remove(detailInDb);
			}
			await _context.SaveChangesAsync();

			return Ok((new { LikeOrUnlike = like, LikeCount = article.FArticleLikes }).ToJson());
		}

		// GET: TArticles/Create
		public IActionResult Create()
		{
			//if (GetUserId() == 0) return RedirectToAction(nameof(HomeController.Login));

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
			//if (GetUserId() == 0) return RedirectToAction(nameof(HomeController.Login));

			if (ModelState.IsValid)
			{
				if (GetUserId() != 0) vm.FMemberId = GetUserId();

				await repo.CreateAsync(vm);
				return RedirectToAction(nameof(Index));
			}
			ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", vm.FArticleCategoryId);
			ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName", vm.FMemberId);
			return View(vm);
		}

		// GET: TArticles/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			//if (GetUserId() == 0) return RedirectToAction(nameof(HomeController.Login));

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
			//if (GetUserId() == 0) return RedirectToAction(nameof(HomeController.Login));

			//if (id != vm.FArticleId)
			//{
			//    return NotFound();
			//}

			if (ModelState.IsValid)
			{
				try
				{
					if (GetUserId() != 0) vm.FMemberId = GetUserId();

					await repo.UpdateAsync(vm);
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

		// POST: TArticles/Delete/5
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteAjax(int articleId)
		{
			//if (GetUserId() == 0) return RedirectToAction(nameof(HomeController.Login));

			if (_context.TArticles == null)
			{
				return Problem("Entity set 'InseparableContext.TArticles'  is null.");
			}
			var article = await _context.TArticles.FindAsync(articleId);

			if (article != null)
			{
				repo.Delete(article.FArticleId);
			}

			return RedirectToAction(nameof(Index));
		}

		private bool TArticlesExists(int id)
		{
			return (_context.TArticles?.Any(e => e.FArticleId == id)).GetValueOrDefault();
		}
	}
}
