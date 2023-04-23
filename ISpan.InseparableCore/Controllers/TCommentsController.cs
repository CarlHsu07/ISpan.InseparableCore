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

namespace ISpan.InseparableCore.Controllers
{
    public class TCommentsController : Controller
    {
        private readonly InseparableContext _context;
		private readonly CommentRepository repo;
        public TCommentsController(InseparableContext context)
        {
            _context = context;
			repo = new CommentRepository(context);
        }

		[HttpPost]
        public async Task<IActionResult> Index(int articleId)
        {
            var comments = repo.Search(articleId);
            return View(comments);
        }
		[HttpPost]
		public async Task<IActionResult> ArticleComment(CommentVm comment)
		{
			List<CommentVm> vms = new List<CommentVm>();
			//無參數=>預設顯示
			if (comment.FMemberId == 0 || string.IsNullOrEmpty(comment.FCommentContent))
			{
				repo.Search(comment.FArticleId);
			}
			else if (comment.FCommentId != 0 || comment.FDeleted)//comment已存在=>跟新or刪除
			{
				repo.UpdateAsync(comment);
			}
			else // 新comment=>新增
			{
				await repo.CreateAsync(comment);
			}

			var comments = _context.TComments.Where(t => t.FArticleId == comment.FArticleId).ToList();
			vms = repo.ModelToVms(comments).ToList();

			return Ok(vms.ToJson());
		}


        private bool TCommentsExists(int id)
        {
          return (_context.TComments?.Any(e => e.FCommentId == id)).GetValueOrDefault();
        }
    }
}
