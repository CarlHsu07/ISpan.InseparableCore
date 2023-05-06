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
				var member = context.TMembers.FirstOrDefault(t => t.FId == comment.FMemberId);
				vm.MemberName = member.FLastName + member.FFirstName;
				vm.FMemberId = member.FMemberId;
				vms.Add(vm);
			}
			return vms;
		}

		public CommentVm GetVmById(int id)
		{
			TComments comment = context.TComments.Find(id);
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
		public void Update(CommentVm vm)
		{
			TComments comment = context.TComments.Find(vm.FCommentId);
			comment.FCommentContent = vm.FCommentContent;
			comment.FDeleted = vm.FDeleted;
			context.Update(comment);
			context.SaveChanges();
		}
	}
}
