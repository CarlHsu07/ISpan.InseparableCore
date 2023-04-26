using ISpan.InseparableCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ISpan.InseparableCore.Models.DAL
{
	public class CommentRepository
	{
		private readonly InseparableContext context;

		public CommentRepository(InseparableContext context)
		{
			this.context = context;
		}

		public IEnumerable<CommentVm> Search(int articleId)
		{
			var comments = context.TComments.Where(t => t.FArticleId == articleId)
				.OrderByDescending(t => t.FCommentPostingDate).ToList();

			return ModelToVms(comments);
		}
		public IEnumerable<CommentVm> ModelToVms(IEnumerable<TComments> comments)
		{
			List<CommentVm> vms = new List<CommentVm>();
			foreach (var comment in comments)
			{
				CommentVm vm = comment.ModelToVm();
				vm.MemberName = context.TComments.Include(t => t.FMember)
					.FirstOrDefault(t => t.FCommentId == vm.FCommentId).FMember.FFirstName;
				vms.Add(vm);
			}
			return vms;
		}

		public CommentVm GetVmById(int id)
		{
			TComments comment = context.TComments.FirstOrDefault(t => t.FCommentId == id);
			CommentVm vm = comment.ModelToVm();
			return vm;
		}
		public async Task CreateAsync(CommentVm vm)
		{
			//新增Comment
			vm.FCommentPostingDate = DateTime.Now;
			vm.FCommentModifiedDate = DateTime.Now;
			TComments comment = vm.VmToModel();

			context.Add(comment);

			await context.SaveChangesAsync();
		}
		public async Task UpdateAsync(CommentVm vm)
		{
			TComments comment = context.TComments.FirstOrDefault(t => t.FCommentId == vm.FCommentId);
			comment.FCommentContent = vm.FCommentContent;
			comment.FDeleted = vm.FDeleted;
			context.Update(comment);
			await context.SaveChangesAsync();

		}
	}
}
