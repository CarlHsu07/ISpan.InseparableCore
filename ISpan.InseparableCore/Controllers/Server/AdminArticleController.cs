using ISpan.InseparableCore.Models.DAL.Repo;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using NuGet.Protocol;

namespace ISpan.InseparableCore.Controllers.Server
{
	public class AdminArticleController : Controller
	{
		private readonly InseparableContext _context;
		private readonly ArticleRepository articleRepo;
		private readonly ArticleLikeRepository likeRepo;
		public AdminArticleController(InseparableContext context)
		{
			_context = context;
			articleRepo = new ArticleRepository(context);
			likeRepo = new ArticleLikeRepository(context);
		}

		//產生頁碼
		protected IPagedList<ArticleVm> GetPagedProcess(int? page, int pageSize, List<ArticleVm> articles)
		{
			// 過濾從client傳送過來有問題頁數
			if (page.HasValue && page < 1) return null;
			// 從資料庫取得資料
			var listUnpaged = articles;
			IPagedList<ArticleVm> pagelist = listUnpaged.ToPagedList(page ?? 1, pageSize);
			// 過濾從client傳送過來有問題頁數，包含判斷有問題的頁數邏輯
			if (pagelist.PageNumber != 1 && page.HasValue && page > pagelist.PageCount) return null;
			return pagelist;
		}
		// GET: TArticles
		public async Task<IActionResult> IndexMaintainer()
		{
			List<ArticleVm> articles = articleRepo.Search(null).ToList();

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
		public async Task<IActionResult> IndexMaintainer(ArticleSearchCondition condition)
		{
			List<ArticleVm> articles = articleRepo.Search(condition).ToList();

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
		public async Task<IActionResult> AdminDetails(int? id)
		{
			if (id == null || _context.TArticles == null)
			{
				return NotFound();
			}

			var vm = articleRepo.GetVmById((int)id);

			return View(vm);
		}

	}
}
