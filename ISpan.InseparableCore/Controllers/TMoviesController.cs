using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.ViewModels;
using NuGet.Protocol;
using ISpan.InseparableCore.Models.DAL;
using AspNetCore;

namespace ISpan.InseparableCore.Controllers
{
	public class TMoviesController : Controller
	{
		private readonly InseparableContext _context;
		private readonly IWebHostEnvironment _enviro;
		private readonly MovieRepository repo;

		public TMoviesController(InseparableContext context, IWebHostEnvironment enviro)
		{
			_context = context;
			this._enviro = enviro;
			repo = new MovieRepository(context, enviro);
		}

		// GET: TMovies
		public async Task<IActionResult> IndexMaintain()
		{
			var inseparableContext = _context.TMovies.Include(t => t.FMovieLevel);

			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			return View(await inseparableContext.ToListAsync());
		}
		public IActionResult Index()
		{
			List<MovieVm> movies = repo.Search(null).ToList();

			int pageContent = 2;
			int pageNumber = movies.Count % pageContent == 0 ? movies.Count / pageContent
														   : movies.Count / pageContent + 1;
			List<Page> pageSelectList = new List<Page>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new Page { Id = i, Value = i });
			}
			movies = movies.Take(pageContent).ToList();
			ViewData["Page"] = new SelectList(pageSelectList, "Id", "Value");

			TMovieCategories defaultCategory = new TMovieCategories() { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", 0);

			//為電影等級SelectList加入預設值
			TMovieLevels defaultLevel = new TMovieLevels { FLevelId = 0, FLevelName = "全部" };
			List<TMovieLevels> LevelSelectList = _context.TMovieLevels.ToList();
			LevelSelectList.Add(defaultLevel);
			ViewData["LevelId"] = new SelectList(LevelSelectList, "FLevelId", "FLevelName", 0);

			//List<string> dateCategories = new List<string> { "全部電影", "熱映中", "即將上映", "已下映" };
			//SelectList dateCategorySelectList = dateCategories.ToSelectList();
			//ViewData["DateCategoryId"] = new SelectList(dateCategorySelectList, "Value", "Text", 0);

			return View(movies);
		}
		[HttpPost]
		public IActionResult Index(MovieSearchCondition condition)
		{
			List<MovieVm> movies = repo.Search(condition).ToList();

			//產生頁碼SelectList
			int pageContent = 2;
			int pageNumber = movies.Count % pageContent == 0 ? movies.Count / pageContent
														   : movies.Count / pageContent + 1;
			List<Page> pageSelectList = new List<Page>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new Page { Id = i, Value = i });
			}
			movies = movies.Skip(pageContent * (condition.Page - 1)).Take(pageContent).ToList();
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

			////為上下映日期SelectList加入預設值
			//List<string> dateCategories = new List<string> { "全部電影", "熱映中", "即將上映", "已下映" };
			//SelectList dateCategorySelectList = dateCategories.ToSelectList();
			//ViewData["DateCategoryId"] = new SelectList(dateCategorySelectList, "Value", "Text", condition.DateCategoryId);

			return Ok(movies.ToJson());
		}


		// GET: TMovies/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.TMovies == null)
			{
				return NotFound();
			}

			MovieVm vm = repo.GetVmById((int)id);
			if (vm == null)
			{
				return NotFound();
			}

			List<string> scoreList = new List<string>();
			for (int i = 0; i <= 10; i++) scoreList.Add(i.ToString());
			
			//SelectList scoreSelectList = scoreList.ToSelectList();
			//ViewData["DateCategoryId"] = new SelectList(scoreSelectList, "Value", "Text");

			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> Details(TMovieScoreDetails detail)
		{
			detail.FMemberId = 1;
			TMovieScoreDetails detailInDb = _context.TMovieScoreDetails.FirstOrDefault(t => t.FMovieId == detail.FMovieId
																		 && t.FMemberId == detail.FMemberId);
			if (detailInDb == null) _context.Add(detail);
			else
			{
				detailInDb.FScore = detail.FScore;
				_context.Update(detailInDb);
			}
			return Ok();
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
				return RedirectToAction(nameof(Index));
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

			var tMovies = await _context.TMovies.FindAsync(id);
			if (tMovies == null)
			{
				return NotFound();
			}
			MovieVm vm = tMovies.ModelToVm();
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
			//if (id != vm.FMovieId)
			//{
			//	return NotFound();
			//}

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
				return RedirectToAction(nameof(Index));
			}
			ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", vm.FMovieLevelId);
			return View(vm);
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
				IEnumerable<TMovieCategoryDetails> categoryDetails = _context.TMovieCategoryDetails.Where(t => t.FMovieId == tMovies.FMovieId);
				foreach (TMovieCategoryDetails detail in categoryDetails)
				{
					_context.Remove(detail);
				}
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
