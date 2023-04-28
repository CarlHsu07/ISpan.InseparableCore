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
using X.PagedList;
using prjMvcCoreDemo.Models;
using System.Text.Json;
using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.BLL.DTOs;

namespace ISpan.InseparableCore.Controllers
{
	public class TMoviesController : Controller
	{
		private readonly InseparableContext _context;
		private readonly IWebHostEnvironment _enviro;
		private readonly MovieRepository repo;
		private readonly MovieService service;
		public TMoviesController(InseparableContext context, IWebHostEnvironment enviro)
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
		public IActionResult Index()
		{
			int pageSize = 10;
			List<MovieSearchDto> dtos = service.Search(null).ToList();
			ViewBag.MovieModel = GetPage.GetPagedProcess(1, pageSize, dtos);

			dtos = dtos.Take(pageSize).ToList();
			List<MovieSearchVm> vms = DtosToVms(dtos).ToList();

			#region ViewData
			int pageContent = 2;
			int pageNumber = dtos.Count % pageContent == 0 ? dtos.Count / pageContent
														   : dtos.Count / pageContent + 1;
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
		public IActionResult Index(MovieSearchCondition condition)
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
			int pageNumber = dtos.Count % pageContent == 0 ? dtos.Count / pageContent
														   : dtos.Count / pageContent + 1;
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
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.TMovies == null)
			{
				return NotFound();
			}

			var dto = service.GetSearchDto((int)id);
			var vm = dto.SearchDtoToVm();
			vm.Categories = repo.GetCategories(dto.FMovieId);
			vm.Level = repo.GetMovieLevel(dto.FMovieLevelId);

			return View(vm);
		}
	}
}
