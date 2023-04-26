using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleLikeVm
	{
		public int FSerialNumber { get; set; }
		public int FMemberId { get; set; }
		public int FArticleId { get; set; }

	}
	public static class ArticleLikeVmExtensions
	{
		public static ArticleLikeVm ModelToVm(this TArticleLikeDetails detail)
		{
			return new ArticleLikeVm()
			{
				FSerialNumber = detail.FSerialNumber,
				FMemberId = detail.FMemberId,
				FArticleId = detail.FArticleId,
			};
		}
		public static TArticleLikeDetails VmToModel(this ArticleLikeVm vm)
		{
			return new TArticleLikeDetails()
			{
				FSerialNumber = vm.FSerialNumber,
				FMemberId = vm.FMemberId,
				FArticleId = vm.FArticleId,
			};
		}
	}

}
