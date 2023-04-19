﻿using ISpan.InseparableCore.ViewModels;
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
			var comments = context.TComments.Where(t=>t.FArticleId == articleId).ToList();

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
			context.SaveChanges();

			await context.SaveChangesAsync();
		}
		public async Task UpdateAsync(CommentVm vm)
		{
			vm.FCommentModifiedDate = DateTime.Now;
			TComments comment = vm.VmToModel();

			context.Update(comment);
			await context.SaveChangesAsync();

		}
		public async Task DeleteAsync(int commentId)
		{
			var comment = await context.TComments.FindAsync(commentId);
			if (comment != null)
			{
				context.TComments.Remove(comment);
			}
		}

	}
}