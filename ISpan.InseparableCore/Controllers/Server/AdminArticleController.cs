using ISpan.InseparableCore.Models.DAL.Repo;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using NuGet.Protocol;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.BLL;

namespace ISpan.InseparableCore.Controllers.Server
{
	public class AdminArticleController : AdminSuperController
	{
		private readonly InseparableContext _context;
		private readonly ArticleRepository articleRepo;
		private readonly ArticleService articleService;
		private readonly ArticleLikeRepository likeRepo;
		public AdminArticleController(InseparableContext context)
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
			return RedirectToAction(nameof(IndexMaintainer), new { errorMessage });
		}

		// GET: TArticles
		public async Task<IActionResult> IndexMaintainer(string errorMessage = "")
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
		public async Task<IActionResult> IndexMaintainer(ArticleSearchCondition condition)
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
		public async Task<IActionResult> AdminDetails(int? id)
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

			return View(vm);
		}
		public async Task<IActionResult> Delete(int articleId)
		{
			if (_context.TArticles == null)
			{
				return Problem("Entity set 'InseparableContext.TArticles'  is null.");
			}

			try
			{
				articleRepo.Delete(articleId);
			}
			catch (Exception ex)
			{
				return ShowError(ex);
			}

			return RedirectToAction(nameof(IndexMaintainer));
		}

	}
}
