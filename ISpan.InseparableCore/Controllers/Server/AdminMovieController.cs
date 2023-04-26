using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using X.PagedList;

namespace ISpan.InseparableCore.Controllers.Server
{
	public class AdminMovieController : Controller
	{
		private readonly InseparableContext _context;
		private readonly IWebHostEnvironment _enviro;
		private readonly MovieRepository repo;

		public AdminMovieController(InseparableContext context, IWebHostEnvironment enviro)
		{
			_context = context;
			this._enviro = enviro;
			repo = new MovieRepository(context, enviro);
		}

		//產生頁碼
		protected IPagedList<MovieVm> GetPagedProcess(int? page, int pageSize, List<MovieVm> movies)
		{
			// 過濾從client傳送過來有問題頁數
			if (page.HasValue && page < 1)
				return null;
			// 從資料庫取得資料
			var listUnpaged = movies;
			IPagedList<MovieVm> pagelist = listUnpaged.ToPagedList(page ?? 1, pageSize);
			// 過濾從client傳送過來有問題頁數，包含判斷有問題的頁數邏輯
			if (pagelist.PageNumber != 1 && page.HasValue && page > pagelist.PageCount)
				return null;
			return pagelist;
		}

		// GET: TMovies
		public async Task<IActionResult> IndexMaintainer()
		{
			List<MovieVm> movies = repo.Search(null).ToList();

			#region ViewData
			int pageContent = 2;
			int pageNumber = movies.Count % pageContent == 0 ? movies.Count / pageContent
														   : movies.Count / pageContent + 1;
			List<SelectListItem> pageSelectList = new List<SelectListItem>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new SelectListItem(i.ToString(), i.ToString()));
			}
			ViewData["Page"] = new SelectList(pageSelectList, "Value", "Text");

			TMovieCategories defaultCategory = new TMovieCategories() { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", 0);

			//為電影等級SelectList加入預設值
			TMovieLevels defaultLevel = new TMovieLevels { FLevelId = 0, FLevelName = "全部" };
			List<TMovieLevels> LevelSelectList = _context.TMovieLevels.ToList();
			LevelSelectList.Add(defaultLevel);
			ViewData["LevelId"] = new SelectList(LevelSelectList, "FLevelId", "FLevelName", 0);

			List<string> dateCategories = new List<string> { "全部電影", "熱映中", "即將上映", "已下映" };
			SelectList dateCategorySelectList = dateCategories.ToSelectList();
			ViewData["DateCategoryId"] = new SelectList(dateCategorySelectList, "Value", "Text", 0);
			#endregion
			int pageSize = 10;

			ViewBag.MovieModel = GetPagedProcess(1, pageSize, movies);

			movies = movies.Take(pageSize).ToList();
			return View(movies);
		}
		[HttpPost]
		public IActionResult IndexMaintainer(MovieSearchCondition condition)
		{
			List<MovieVm> movies = repo.Search(condition).ToList();

			#region ViewData

			//產生頁碼SelectList
			int pageContent = 2;
			int pageNumber = movies.Count % pageContent == 0 ? movies.Count / pageContent
														   : movies.Count / pageContent + 1;
			List<SelectListItem> pageSelectList = new List<SelectListItem>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new SelectListItem(i.ToString(), i.ToString()));
			}
			ViewData["Page"] = new SelectList(pageSelectList, "Id", "Value", condition.Page);

			//為電影類別SelectList加入預設值
			TMovieCategories defaultCategory = new TMovieCategories { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", condition.CategoryId);

			//為電影等級SelectList加入預設值
			TMovieLevels defaultLevel = new TMovieLevels { FLevelId = 0, FLevelName = "全部" };
			List<TMovieLevels> LevelSelectList = _context.TMovieLevels.ToList();
			LevelSelectList.Add(defaultLevel);
			ViewData["LevelId"] = new SelectList(LevelSelectList, "FLevelId", "FLevelName", condition.LevelId);

			//為上下映日期SelectList加入預設值
			List<string> dateCategories = new List<string> { "全部電影", "熱映中", "即將上映", "已下映" };
			SelectList dateCategorySelectList = dateCategories.ToSelectList();
			ViewData["DateCategoryId"] = new SelectList(dateCategorySelectList, "Value", "Text", condition.DateCategoryId);
			#endregion
			int pageSize = 10;
			var pageList = GetPagedProcess(condition.Page, pageSize, movies);
			movies = movies.Skip(pageSize * (condition.Page - 1)).Take(pageSize).ToList();
			if (movies.Count == 0) return Ok("noData");

			return Ok(new
			{
				Vm = movies,
				PageCount = pageList.PageCount,
				TotalItemCount = pageList.TotalItemCount,
				PageSize = pageSize
			}.ToJson());
		}


		// GET: TMovies/Details/5
		public async Task<IActionResult> AdminDetails(int? id)
		{
			if (id == null || _context.TMovies == null)
			{
				return NotFound();
			}

			MovieVm vm = repo.GetVmById((int)id);
			return View(vm);
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
		public async Task<IActionResult> Create(MovieVm vm)
		{
			if (ModelState.IsValid)
			{
				await repo.CreateAsync(vm);
				return RedirectToAction(nameof(IndexMaintainer));
			}
			ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", vm.FMovieLevelId);
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			return View(vm);
		}

		// GET: TMovies/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.TMovies == null)
			{
				return NotFound();
			}

			var movie = await _context.TMovies.FindAsync(id);
			if (movie == null)
			{
				return NotFound();
			}
			MovieVm vm = movie.ModelToVm();
			ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", vm.FMovieLevelId);
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			return View(vm);
		}

		// POST: TMovies/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, MovieVm vm)
		{
			if (id != vm.FMovieId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					await repo.UpdateAsync(vm);
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TMoviesExists(vm.FMovieId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(IndexMaintainer));
			}
			ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", vm.FMovieLevelId);
			return View(vm);
		}

		// POST: TMovies/Delete/5
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int movieId)
		{
			if (_context.TMovies == null)
			{
				return Problem("Entity set 'InseparableContext.TMovies'  is null.");
			}
			var movie = await _context.TMovies.FindAsync(movieId);

			await repo.DeleteAsync(movieId);

			return RedirectToAction(nameof(IndexMaintainer));
		}
		[HttpPost]
		public async Task<IActionResult> DeleteAjax(int movieId)
		{
			if (_context.TMovies == null)
			{
				return Problem("Entity set 'InseparableContext.TMovies'  is null.");
			}
			var movie = await _context.TMovies.FindAsync(movieId);
			await repo.DeleteAsync(movieId);
			return Ok();
		}

		private bool TMoviesExists(int id)
		{
			return (_context.TMovies?.Any(e => e.FMovieId == id)).GetValueOrDefault();
		}

	}
}
