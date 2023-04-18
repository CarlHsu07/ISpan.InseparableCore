using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleVm
	{
		public int FArticleId { get; set; }
		[DisplayName("標題")]
		public string? FArticleTitle { get; set; }
		public int FMemberId { get; set; }
		[DisplayName("發文者")]
		public string? MemberName { get; set; }
		public int FArticleCategoryId { get; set; }
		[DisplayName("文章類別")]
		public string? ArticleCategory { get; set; }
		[DisplayName("發文時間")]
		public DateTime? FArticlePostingDate { get; set; }
		[DisplayName("發文時間")]
		public string? PostingDate { get; set; }
		[DisplayName("修改時間")]
		public DateTime? FArticleModifiedDate { get; set; }
		[DisplayName("修改時間")]
		public string? ModifiedDate { get; set; }
		[DisplayName("點讚數")]
		public int FArticleLikes { get; set; }
		[DisplayName("點閱數")]
		public int FArticleClicks { get; set; }
		[DisplayName("文章內容")]
		public string? FArticleContent { get; set; }
		[DisplayName("文章內容")]
		public string? PartialContent { get; set; }
		public bool FDeleted { get; set; } = false;
		public int Page { get; set; }
	}
	public static class ArticleVmExtension
	{
		public static ArticleVm ModelToVm(this TArticles article)
		{
			int len = Math.Min(article.FArticleContent.Length, 10);
			return new ArticleVm()
			{
				FArticleId = article.FArticleId,
				FArticleTitle = article.FArticleTitle,
				FMemberId = article.FMemberId,
				FArticlePostingDate = article.FArticlePostingDate,
				PostingDate = article.FArticlePostingDate.ToString("yyyy-MM-dd HH:mm:ss"),
				FArticleModifiedDate = article.FArticleModifiedDate,
				ModifiedDate = article.FArticleModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"),
				FArticleLikes = article.FArticleLikes,
				FArticleClicks = article.FArticleClicks,
				FArticleContent = article.FArticleContent,
				PartialContent = article.FArticleContent.Substring(0, len) + "...",
				FArticleCategoryId = article.FArticleCategoryId,
				FDeleted = article.FDeleted,
			};
		}

		public static TArticles VmToModel(this ArticleVm vm)
		{
			return new TArticles()
			{
				FArticleId = vm.FArticleId,
				FArticleTitle = vm.FArticleTitle,
				FMemberId = vm.FMemberId,
				FArticlePostingDate = (DateTime)vm.FArticlePostingDate,
				FArticleModifiedDate = (DateTime)vm.FArticleModifiedDate,
				FArticleLikes = vm.FArticleLikes,
				FArticleClicks = vm.FArticleClicks,
				FArticleContent = vm.FArticleContent,
				FArticleCategoryId = vm.FArticleCategoryId,
				FDeleted = vm.FDeleted,
			};
		}

	}
}
