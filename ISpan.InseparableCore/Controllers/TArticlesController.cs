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
using ISpan.InseparableCore.Models.DAL.Repo;
using ISpan.InseparableCore.Models.BLL;
using ISpan.InseparableCore.Models.BLL.DTOs;
using System.Buffers;
using Humanizer;

namespace ISpan.InseparableCore.Controllers
{
	public class TArticlesController : SuperController
	{
		private readonly InseparableContext _context;
		private readonly ArticleRepository articleRepo;
		private readonly ArticleService articleService;
		private readonly ArticleLikeRepository likeRepo;
		public TArticlesController(InseparableContext context)
		{
			_context = context;
			articleRepo = new ArticleRepository(context);
			articleService = new ArticleService(articleRepo);
			likeRepo = new ArticleLikeRepository(context);
		}
		public IEnumerable<ArticleSearchVm> DtosToVms(IEnumerable<ArticleSearchDto> dtos)
		{
			List<ArticleSearchVm> vms = new List<ArticleSearchVm>();

			foreach (var dto in dtos)
			{
				var vm = dto.SearchDtoToVm();
				vm.ArticleCategory = articleRepo.GetCategory(dto.FArticleCategoryId);
				var member = articleRepo.GetMemberByPK(dto.FMemberId);
				vm.FMemberId = member.FMemberId;
				vm.MemberName = member.FLastName + member.FFirstName;
				vms.Add(vm);
			}
			return vms;
		}
		private IActionResult ShowError(Exception ex)
		{
			string errorMessage = ex.Message;
			return RedirectToAction(nameof(Index), new { errorMessage });
		}

		// GET: TArticles
		public async Task<IActionResult> Index(string errorMessage = "")
		{
			int pageSize = 10;
			List<ArticleSearchDto> dtos = articleService.Search(null).ToList();

			ViewBag.ArticleModel = GetPage.GetPagedProcess(1, pageSize, dtos);
			dtos = dtos.Take(pageSize).ToList();
			var vms = DtosToVms(dtos);
			#region ViewData
			int pageContent = 2;
			int pageNumber = dtos.Count % pageContent == 0 ? dtos.Count / pageContent
														   : dtos.Count / pageContent + 1;
			List<SelectListItem> pageSelectList = new List<SelectListItem>();
			for (int i = 1; i < pageNumber + 1; i++)
			{
				pageSelectList.Add(new SelectListItem(i.ToString(), i.ToString()));
			}
			//dtos = dtos.Take(pageContent).ToList();
			ViewData["Page"] = new SelectList(pageSelectList, "Value", "Text");

			TMovieCategories defaultCategory = new TMovieCategories() { FMovieCategoryId = 0, FMovieCategoryName = "全部" };
			List<TMovieCategories> categorySelectList = _context.TMovieCategories.ToList();
			categorySelectList.Add(defaultCategory);
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", 0);
			#endregion
			ViewBag.errorMessage = errorMessage;

			return View(vms);
		}
		[HttpPost]
		public async Task<IActionResult> Index(ArticleSearchCondition condition)
		{
			int pageSize = 10;
			List<ArticleSearchDto> dtos = articleService.Search(condition).ToList();

			var pageList = GetPage.GetPagedProcess(condition.Page, pageSize, dtos);
			dtos = dtos.Skip(pageSize * ((int)condition.Page - 1)).Take(pageSize).ToList();
			if (dtos.Count == 0) return Ok("noData");
			var vms = DtosToVms(dtos);

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
			ViewData["FMovieCategoryId"] = new SelectList(categorySelectList, "FMovieCategoryId", "FMovieCategoryName", condition.CategoryId);
			#endregion

			return Ok(new
			{
				Vm = vms,
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
			var dto = new ArticleSearchDto();
			try
			{
				dto = articleService.GetSearchDto((int)id);
			}
			catch (Exception ex)
			{
				return ShowError(ex);
			}

			ArticleSearchVm vm = dto.SearchDtoToVm();
			vm.ArticleCategory = articleRepo.GetCategory(dto.FArticleCategoryId);
			var member = articleRepo.GetMemberByPK(dto.FMemberId);
			vm.FMemberId = member.FMemberId;
			vm.MemberName = member.FLastName + member.FFirstName;

			//是否點讚
			vm.LikeOrUnlike = likeRepo.LikeOrNot((int)id, _user.FId);

			//點閱數+1
			articleService.Click(vm.FArticleId);

			ViewBag.UserId = _user.FId;

			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> ArticleLike(ArticleLikeVm vm)
		{
			vm.FMemberId = _user.FId;
			bool like = false;

			var detailInDb = likeRepo.GetLikeVm(vm.FArticleId, vm.FMemberId);

			int articleLikes = articleService.getArticleLikes(vm.FArticleId);

			if (detailInDb == null)
			{
				articleLikes++;
				await articleRepo.UpdateLikes(vm.FArticleId, articleLikes);

				like = true;
				likeRepo.Create(vm);
			}
			else
			{
				articleLikes--;
				await articleRepo.UpdateLikes(vm.FArticleId, articleLikes);

				like = false;
				likeRepo.Delete(detailInDb.FSerialNumber);
			}

			return Ok((new 
			{ 
				LikeOrUnlike = like,
				LikeCount = articleLikes 
			}).ToJson());
		}

		// GET: TArticles/Create
		public IActionResult Create()
		{
			ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			ViewBag.UserId = _user.FId;
			return View();
		}

		// POST: TArticles/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(ArticleCreateVm vm)
		{
			if (ModelState.IsValid)
			{
				var dto = vm.CreateVmToDto();
				articleService.Create(dto);
				return RedirectToAction(nameof(Index));
			}
			ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", vm.FArticleCategoryId);
			ViewBag.UserId = _user.FId;

			return View(vm);
		}

		// GET: TArticles/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.TArticles == null)
			{
				return NotFound();
			}
			var dto = new ArticleUpdateDto();
			try
			{
				dto = articleService.GetUpdateDto((int)id);
			}
			catch (Exception ex)
			{
				return ShowError(ex);
			}

			var vm = dto.UpdateDtoToVm();
			ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", vm.FArticleCategoryId);
			return View(vm);
		}

		// POST: TArticles/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, ArticleUpdateVm vm)
		{
			//if (id != vm.FArticleId)
			//{
			//    return NotFound();
			//}

			if (ModelState.IsValid)
			{
				try
				{
					var dto = vm.UpdateVmToDto();
					articleService.Update(dto);
				}
				catch (Exception ex)
				{
					ShowError(ex);
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["FArticleCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", vm.FArticleCategoryId);
			return View(vm);
		}

		// POST: TArticles/Delete/5
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public IActionResult Delete(int articleId)
		{
			if (_context.TArticles == null)
			{
				return Problem("Entity set 'InseparableContext.TArticles'  is null.");
			}
			articleRepo.Delete(articleId);

			return RedirectToAction(nameof(Index));
		}
		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteAjax(int articleId)
		{
			if (_context.TArticles == null)
			{
				return Problem("Entity set 'InseparableContext.TArticles'  is null.");
			}
			await articleRepo.Delete(articleId);

			return RedirectToAction(nameof(Index));
		}

		private bool TArticlesExists(int id)
		{
			return (_context.TArticles?.Any(e => e.FArticleId == id)).GetValueOrDefault();
		}
	}
}
