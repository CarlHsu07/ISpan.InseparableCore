using ISpan.InseparableCore.ViewModels;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
	public class ArticleLikeRepository
	{
		private readonly InseparableContext context;

		public ArticleLikeRepository(InseparableContext context)
		{
			this.context = context;
		}
		public ArticleLikeVm? GetLikeVm(int articleId, int userId)
		{
			var detailInDb = context.TArticleLikeDetails
				.FirstOrDefault(t => t.FMemberId == userId
									 && t.FArticleId == articleId);
			return detailInDb == null ? null : detailInDb.ModelToVm();
		}
		public bool LikeOrNot(int articleId, int userId)
		{
			var detailInDb = context.TArticleLikeDetails
				.FirstOrDefault(t => t.FMemberId == userId
								  && t.FArticleId == articleId);
			return detailInDb != null;
		}
		public void Create(ArticleLikeVm vm)
		{
			var detail = vm.VmToModel();
			context.Add(detail);
			context.SaveChanges();
		}
		public void Delete(int serialNumber)
		{
			var detail = context.TArticleLikeDetails
				.FirstOrDefault(t => t.FSerialNumber == serialNumber);

			context.Remove(detail);
			context.SaveChanges();
		}
	}
}
