using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.BLL.DTOs;
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
		private readonly MovieService service;

		public AdminMovieController(InseparableContext context, IWebHostEnvironment enviro)
		{
			_context = context;
			this._enviro = enviro;
			repo = new MovieRepository(context, enviro);
			service = new MovieService(repo);
		}

		public IEnumerable<MovieSearchVm> DtosToVms(IEnumerable<MovieSearchDto> dtos)
		{
			List<MovieSearchVm> vms = new List<MovieSearchVm>();

			foreach (var dto in dtos)
			{
				var vm = dto.SearchDtoToVm();
				vm.Categories = repo.GetCategories(dto.FMovieId);
				vm.Level = repo.GetMovieLevel(dto.FMovieLevelId);
				vms.Add(vm);
			}
			return vms;
		}

		// GET: TMovies
		public async Task<IActionResult> IndexMaintainer()
		{
			int pageSize = 10;
			List<MovieSearchDto> dtos = service.Search(null).ToList();
			ViewBag.MovieModel = GetPage.GetPagedProcess(1, pageSize, dtos);

			dtos = dtos.Take(pageSize).ToList();
			List<MovieSearchVm> vms = DtosToVms(dtos).ToList();

			#region ViewData
			int pageContent = 2;
			int pageNumber = vms.Count % pageContent == 0 ? vms.Count / pageContent
														   : vms.Count / pageContent + 1;
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
			return View(vms);
		}
		[HttpPost]
		public IActionResult IndexMaintainer(MovieSearchCondition condition)
		{
			List<MovieSearchDto> dtos = service.Search(condition).ToList();
			int pageSize = 10;
			var pageList = GetPage.GetPagedProcess(condition.Page, pageSize, dtos);
			dtos = dtos.Skip(pageSize * (condition.Page - 1)).Take(pageSize).ToList();
			if (dtos.Count == 0) return Ok("noData");
			List<MovieSearchVm> vms = DtosToVms(dtos).ToList();

			#region ViewData

			//產生頁碼SelectList
			int pageContent = 2;
			int pageNumber = vms.Count % pageContent == 0 ? vms.Count / pageContent
														   : vms.Count / pageContent + 1;
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

			return Ok(new
			{
				Vm = vms,
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

			MovieSearchDto dto = service.GetSearchDto((int)id);
			if (dto == null) return RedirectToAction(nameof(IndexMaintainer));
			var vm = repo.GetMovieVm((int)id);

			return View(vm);
		}

		public string GetImagePath(IFormFile image)
		{
			string imageName = Guid.NewGuid().ToString() + ".jpg";
			string path = _enviro.WebRootPath + "/images/" + imageName;
			image.CopyTo(new FileStream(path, FileMode.Create));
			return path;
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
		public async Task<IActionResult> Create(MovieCreateVm vm)
		{
			var dto = vm.CreateVmToDto();
			if (vm.Image != null) dto.FMovieImagePath = GetImagePath(vm.Image);

			if (ModelState.IsValid)
			{
				service.Create(dto);
				int movieId = repo.GetMovieId(dto.FMovieName);
				repo.CreateCategoryDetail(movieId, vm.CategoryIds);

				return RedirectToAction(nameof(IndexMaintainer));
			}
			ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", vm.FMovieLevelId);
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			return View(vm);
		}

		// GET: TMovies/Edit/5
		public IActionResult Edit(int? id)
		{
			if (id == null || _context.TMovies == null)
			{
				return NotFound();
			}

			MovieUpdateDto dto = service.GetUpdateDto((int)id);
			MovieUpdateVm vm = dto.UpdateDtoToVm();
			vm.CategoryIdsContained = _context.TMovieCategoryDetails.Where(t => t.FMovieId == id).Select(t => t.FMovieCategoryId).ToList();

			ViewData["FMovieLevelId"] = new SelectList(_context.TMovieLevels, "FLevelId", "FLevelName", vm.FMovieLevelId);
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			return View(vm);
		}

		// POST: TMovies/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(int id, MovieUpdateVm vm)
		{
			if (id != vm.FMovieId)
			{
				return NotFound();
			}
			if (vm.Image != null) vm.FMovieImagePath = GetImagePath(vm.Image);

			var dto = vm.UpdateVmToDto();

			if (ModelState.IsValid)
			{
				try
				{
					service.Update(dto);
					repo.UpdateCategoryDetail(dto.FMovieId, vm.CategoryIds);
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
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");

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

			await repo.Delete(movieId);

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
			await repo.Delete(movieId);
			return Ok();
		}

		private bool TMoviesExists(int id)
		{
			return (_context.TMovies?.Any(e => e.FMovieId == id)).GetValueOrDefault();
		}

	}
}
