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
using static System.Formats.Asn1.AsnWriter;

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
			List<SelectListItem> pageSelectList = new List<SelectListItem>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new SelectListItem(i.ToString(), i.ToString()));
			}
			movies = movies.Take(pageContent).ToList();
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
			List<SelectListItem> pageSelectList = new List<SelectListItem>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new SelectListItem(i.ToString(), i.ToString()));
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

			//為上下映日期SelectList加入預設值
			List<string> dateCategories = new List<string> { "全部電影", "熱映中", "即將上映", "已下映" };
			SelectList dateCategorySelectList = dateCategories.ToSelectList();
			ViewData["DateCategoryId"] = new SelectList(dateCategorySelectList, "Value", "Text", condition.DateCategoryId);

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

			ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName");

			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> MovieComment(TMovieCommentDetails comment)
		{
			List<MovieDetailVm> vms = new List<MovieDetailVm>();
			//無參數=>預設顯示
			if (comment.FMemberId == 0 || string.IsNullOrEmpty(comment.FComment))
			{
				vms = _context.TMovieCommentDetails
						.Where(t => t.FMovieId == comment.FMovieId).ToList().ModelToVms();
				foreach (var vm in vms)
				{
					vm.MemberName = _context.TMembers.FirstOrDefault(t => t.FId == vm.FMemberId).FFirstName;
				}
				return Ok(vms.ToJson());
			}
			else if (comment.FSerialNumber != 0)//comment已存在=>跟新
			{
				var commentInDb = await _context.TMovieCommentDetails.FindAsync(comment.FSerialNumber);
				commentInDb.FComment = comment.FComment;
				_context.Update(commentInDb);
				_context.SaveChanges();
			}
			else // 新comment=>新增
			{
				_context.Add(comment);
				_context.SaveChanges();
			}

			vms = _context.TMovieCommentDetails
				.Where(t => t.FMovieId == comment.FMovieId).ToList().ModelToVms();
			foreach (var vm in vms)
			{
				vm.MemberName = _context.TMembers.FirstOrDefault(t => t.FId == vm.FMemberId).FFirstName;
			}
			return Ok(vms.ToJson());
		}

		public async Task<IActionResult> MovieScore(TMovieScoreDetails score)
		{
			TMovieScoreDetails scoreInDb = _context.TMovieScoreDetails.FirstOrDefault(t => t.FMovieId == score.FMovieId && t.FMemberId == score.FMemberId);
			if (scoreInDb == null)
			{
				_context.Add(score);
			}
			else
			{
				scoreInDb.FScore = score.FScore;
				_context.Update(scoreInDb);
			}
			_context.SaveChanges();
			//計算電影評分
			var movie = await _context.TMovies.FindAsync(score.FMovieId);
			movie.FMovieScore = (decimal)_context.TMovieScoreDetails
				.Where(t => t.FMovieId == score.FMovieId).Average(t => t.FScore);
			_context.Update(movie);
			_context.SaveChanges();

			return Ok(movie.FMovieScore.ToString("f1"));
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
			var movie = await _context.TMovies.FindAsync(id);
			if (movie != null)
			{
				IEnumerable<TMovieCategoryDetails> categoryDetails = _context.TMovieCategoryDetails.Where(t => t.FMovieId == movie.FMovieId);
				foreach (TMovieCategoryDetails detail in categoryDetails)
				{
					_context.Remove(detail);
				}
				_context.TMovies.Remove(movie);
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
