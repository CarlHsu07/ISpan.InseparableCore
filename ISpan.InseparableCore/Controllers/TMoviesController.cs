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

namespace ISpan.InseparableCore.Controllers
{
	public class TMoviesController : Controller
	{
		private readonly InseparableContext _context;
		private readonly IWebHostEnvironment enviro;

		public TMoviesController(InseparableContext context, IWebHostEnvironment enviro)
		{
			_context = context;
			this.enviro = enviro;
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
			var inseparableContext = _context.TMovies.ToList();

			List<MovieVm> movies = new List<MovieVm>();
			foreach (var movieModel in inseparableContext)
			{
				MovieVm vm = movieModel.ModelToVm();
				var levelName = _context.TMovies.Include(t => t.FMovieLevel).FirstOrDefault(t => t.FMovieId == vm.FMovieId).FMovieLevel.FLevelName;
				vm.Level = levelName;

				IEnumerable<TMovieCategoryDetails> categorydetails = _context.TMovieCategoryDetails.Where(t => t.FMovieId == vm.FMovieId);
				//_context.Database.CloseConnection();
				if (categorydetails != null)
				{
					List<int> categoryIds = categorydetails.Select(t => t.FMovieCategoryId).ToList();

					List<string> categories = _context.TMovieCategories.Where(t => categoryIds.Contains(t.FMovieCategoryId)).Select(t => t.FMovieCategoryName).ToList();
					vm.Categories = String.Join(", ", categories.ToArray());
				}
				movies.Add(vm);
			}

			TMovieCategories defaultCategory = new TMovieCategories() { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", 0);

			List<string> dateCategories = new List<string> { "全部電影", "熱映中", "即將上映", "已下映" };
			List<MovieDateCategory> dateCategorySelectList = dateCategories.ToSelectList();
			ViewData["DateCategoryId"] = new SelectList(dateCategorySelectList, "Id", "DateCategory", 0);

			return View(movies);
		}
		[HttpPost]
		public IActionResult Index(MovieSearchCondition condition)
		{
			var inseparableContext = _context.TMovies.ToList();

			if (condition.DateCategoryId == 1)//熱映中
			{
				inseparableContext = inseparableContext.Where(t => t.FMovieOnDate < DateTime.Now 
																&& t.FMovieOffDate > DateTime.Now).ToList();
			}
			else if (condition.DateCategoryId == 2)//即將上映
			{
				inseparableContext = inseparableContext.Where(t => t.FMovieOnDate > DateTime.Now).ToList();
			}
			else if (condition.DateCategoryId == 3)//已下映
			{
				inseparableContext = inseparableContext.Where(t => t.FMovieOffDate < DateTime.Now).ToList();
			}

			List<MovieVm> movies = new List<MovieVm>();
			foreach (var movieModel in inseparableContext)
			{
				MovieVm vm = movieModel.ModelToVm();
				var levelName = _context.TMovies.Include(t => t.FMovieLevel).FirstOrDefault(t => t.FMovieId == vm.FMovieId).FMovieLevel.FLevelName;
				vm.Level = levelName;

				IEnumerable<TMovieCategoryDetails> categorydetails = _context.TMovieCategoryDetails.Where(t => t.FMovieId == vm.FMovieId);
				//_context.Database.CloseConnection();
				if (categorydetails != null)
				{

					List<int> categoryIds = categorydetails.Select(t => t.FMovieCategoryId).ToList();

					List<string> categories = _context.TMovieCategories.Where(t => categoryIds.Contains(t.FMovieCategoryId)).Select(t => t.FMovieCategoryName).ToList();
					vm.Categories = String.Join(", ", categories.ToArray());
				}
				movies.Add(vm);
			}

			if (!string.IsNullOrEmpty(condition.Key)) movies = movies.Where(t => t.FMovieName.Contains(condition.Key)).ToList();

			if (condition.CategoryId != 0)
			{
				var movieCategoryDetails = _context.TMovieCategoryDetails.Where(t => t.FMovieCategoryId == condition.CategoryId);
				List<int> movieIds = movieCategoryDetails.Select(t => t.FMovieId).ToList();

				movies = movies.Where(t => movieIds.Contains(t.FMovieId)).ToList();
			}
			movies = movies.Skip(2 * (condition.Page - 1)).Take(2).ToList();
			ViewData["Page"] = condition.Page;

			TMovieCategories defaultCategory = new TMovieCategories { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", 0);

			List<string> dateCategories = new List<string> { "全部電影", "熱映中", "即將上映", "已下映" };
			List<MovieDateCategory> dateCategorySelectList = dateCategories.ToSelectList();
			ViewData["FMovieCategoryId"] = new SelectList(dateCategorySelectList, "Id", "DateCategory", 0);

			return Ok(movies.ToJson());
		}

		// GET: TMovies/Details/5
		public async Task<IActionResult> Details(int? id)
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
		public async Task<IActionResult> Create(MovieVm movieCreateVm)
		{
			if (ModelState.IsValid)
			{
				TMovies movie = movieCreateVm.VmToModel();
				if (movieCreateVm.Image != null)
				{
					string imageName = Guid.NewGuid().ToString() + ".jpg";
					string path = enviro.WebRootPath + "/images/" + imageName;
					movieCreateVm.Image.CopyTo(new FileStream(path, FileMode.Create));
					movie.FMovieImagePath = imageName;
				}
				_context.Add(movie);
				_context.SaveChanges();

				int movieId = _context.TMovies.First(t => t.FMovieName == movieCreateVm.FMovieName).FMovieId;
				if (!string.IsNullOrEmpty(movieCreateVm.CategoryIds))
				{
					List<int> categoryIds = movieCreateVm.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i)).ToList();
					foreach (var id in categoryIds)
					{
						TMovieCategoryDetails detail = new TMovieCategoryDetails()
						{
							FMovieId = movieId,
							FMovieCategoryId = id
						};
						_context.Add(detail);
					}

				}
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", movieCreateVm.FMovieLevelId);
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			return View(movieCreateVm);
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
			//if (id != movieCreateVm.FMovieId)
			//{
			//	return NotFound();
			//}

			if (ModelState.IsValid)
			{
				TMovies movie = vm.VmToModel();
				if (vm.Image != null)
				{
					string imageName = Guid.NewGuid().ToString() + ".jpg";
					string path = enviro.WebRootPath + "/images/" + imageName;
					vm.Image.CopyTo(new FileStream(path, FileMode.Create));
					movie.FMovieImagePath = imageName;
				}

				try
				{
					_context.Update(movie);
					_context.SaveChanges();

					int movieId = _context.TMovies.First(t => t.FMovieName == vm.FMovieName).FMovieId;

					IEnumerable<TMovieCategoryDetails> categoryDetails = _context.TMovieCategoryDetails.Where(t => t.FMovieId == movieId);
					foreach (TMovieCategoryDetails detail in categoryDetails)
					{
						_context.Remove(detail);
					}

					if (!string.IsNullOrEmpty(vm.CategoryIds))
					{
						List<int> categoryIds = vm.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i)).ToList();
						foreach (var categoryId in categoryIds)
						{
							TMovieCategoryDetails detail = new TMovieCategoryDetails()
							{
								FMovieId = movieId,
								FMovieCategoryId = categoryId,
							};
							_context.Add(detail);
						}

					}
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TMoviesExists(movie.FMovieId))
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
