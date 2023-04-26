using ISpan.InseparableCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
	public class MovieCommentRepository
	{
		private readonly InseparableContext context;

		public MovieCommentRepository(InseparableContext context)
		{
			this.context = context;
		}

		public IEnumerable<MovieCommemtVm> Search(int movieId)
		{
			var comments = context.TMovieCommentDetails.Where(t => t.FMovieId == movieId)
				.OrderByDescending(t => t.FPostingDate).ToList();

			return ModelToVms(comments);
		}
		public IEnumerable<MovieCommemtVm> ModelToVms(IEnumerable<TMovieCommentDetails> comments)
		{
			List<MovieCommemtVm> vms = new List<MovieCommemtVm>();
			foreach (var comment in comments)
			{
				MovieCommemtVm vm = comment.ModelToVm();
				vm.MemberName = context.TMovieCommentDetails.Include(t => t.FMember)
					.FirstOrDefault(t => t.FSerialNumber == vm.FSerialNumber).FMember.FFirstName;
				vms.Add(vm);
			}
			return vms;
		}

		public MovieCommemtVm GetVmById(int id)
		{
			TMovieCommentDetails comment = context.TMovieCommentDetails.FirstOrDefault(t => t.FSerialNumber == id);
			MovieCommemtVm vm = comment.ModelToVm();
			return vm;
		}
		public async Task CreateAsync(MovieCommemtVm vm)
		{
			//新增Comment
			vm.FPostingDate = DateTime.Now;
			//vm.FCommentModifiedDate = DateTime.Now;
			TMovieCommentDetails comment = vm.VmToModel();

			context.Add(comment);

			await context.SaveChangesAsync();
		}
		public async Task UpdateAsync(MovieCommemtVm vm)
		{
			TMovieCommentDetails comment = context.TMovieCommentDetails.FirstOrDefault(t => t.FSerialNumber == vm.FSerialNumber);
			comment.FComment = vm.FComment;
			comment.FDeleted = vm.FDeleted;
			context.Update(comment);
			await context.SaveChangesAsync();

		}
	}
}
