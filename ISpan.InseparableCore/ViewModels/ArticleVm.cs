using ISpan.InseparableCore.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleVm
	{
		public TArticles? articles { get; set; }
		public int? FArticleId { get; set; }
		public string? FArticleTitle { get; set; }
		public int? FMemberId { get; set; }
		public int? FArticleCategoryId { get; set; }
		public DateTime FArticlePostingDate { get; set; }
		public int? FArticleLikes { get; set; }
		public int? FArticleClicks { get; set; }
		public string? FArticleContent { get; set; }
		public List<TComments>? Comments { get; set; }
	}
	public static class ArticleVmExtension
	{
		static InseparableContext context = new InseparableContext();
		public static ArticleVm ModelToVm(this TArticles tArticles)
		{
			ArticleVm vm = new ArticleVm()
			{
				articles = tArticles,
				FArticleId = tArticles.FArticleId,
				FArticleTitle = tArticles.FArticleTitle,
				FMemberId = tArticles.FMemberId,
				FArticlePostingDate = tArticles.FArticlePostingDate,
				FArticleLikes = tArticles.FArticleLikes,
				FArticleClicks = tArticles.FArticleClicks,
				FArticleContent = tArticles.FArticleContent,
			};
			//vm.Comments = context.TComments.Where(t => t.FArticleId == tArticles.FArticleId).ToList();
			return vm;
		}
		public static TArticles VmToModel(this ArticleVm vm)
		{
			var movie = new TArticles()
			{
				FArticleId = (int)vm.FArticleId,
				FArticleTitle = vm.FArticleTitle,
				FMemberId = (int)vm.FMemberId,
				FArticlePostingDate = vm.FArticlePostingDate,
				FArticleLikes = (int)vm.FArticleLikes,
				FArticleClicks = (int)vm.FArticleClicks,
				FArticleContent = vm.FArticleContent,
			};
			return movie;
		}

	}
}
