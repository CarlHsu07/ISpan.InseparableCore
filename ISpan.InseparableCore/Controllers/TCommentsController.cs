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

        // GET: TComments
        public async Task<IActionResult> Index()
        {
            var inseparableContext = _context.TComments.Include(t => t.FArticle).Include(t => t.FMember);
            return View(await inseparableContext.ToListAsync());
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
			else if (comment.FCommentId != 0)//comment已存在=>跟新
			{
				var commentInDb = await _context.TComments.FindAsync(comment.FCommentId);
				commentInDb.FCommentContent = comment.FCommentContent;
				repo.UpdateAsync(comment);
			}
			else // 新comment=>新增
			{
				repo.CreateAsync(comment);
			}

			var comments = _context.TComments.Where(t => t.FArticleId == comment.FArticleId).ToList();
			vms = repo.ModelToVms(comments).ToList();

			return Ok(vms.ToJson());
		}

		// GET: TComments/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TComments == null)
            {
                return NotFound();
            }

            var comment = await _context.TComments
                .FirstOrDefaultAsync(m => m.FCommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: TComments/Create
        public IActionResult Create()
        {
            ViewData["FArticleId"] = new SelectList(_context.TArticles, "FArticleId", "FArticleContent");
            ViewData["FMemberId"] = new SelectList(_context.TMembers, "FId", "FFirstName");
            return View();
        }

        // POST: TComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentVm vm)
        {
            if (ModelState.IsValid)
            {
				repo.CreateAsync(vm);
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // POST: TComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CommentVm vm)
        {
            if (id != vm.FCommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					repo.UpdateAsync(vm);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TCommentsExists(vm.FCommentId))
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
            return View(vm);
        }

        // POST: TComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TComments == null)
            {
                return Problem("Entity set 'InseparableContext.TComments'  is null.");
            }
            var tComments = await _context.TComments.FindAsync(id);
            if (tComments != null)
            {
                _context.TComments.Remove(tComments);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TCommentsExists(int id)
        {
          return (_context.TComments?.Any(e => e.FCommentId == id)).GetValueOrDefault();
        }
    }
}
