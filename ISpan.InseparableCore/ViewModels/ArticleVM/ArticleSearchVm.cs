using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Html;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleSearchVm
	{
		[Display(Name = "文章ID")]
		public int FArticleId { get; set; }
		[Display(Name = "標題")]
		public string? FArticleTitle { get; set; }
		[Display(Name = "發文者")]
		public int? FMemberPK { get; set; }
		[Display(Name = "發文ID")]
		public string? FMemberId { get; set; }
		[Display(Name = "發文者")]
		public string? MemberName { get; set; }
		[Display(Name = "類別")]
		public string? ArticleCategory { get; set; }
		[Display(Name = "發文時間")]
		public string? PostingDate { get; set; }
		[Display(Name = "修改時間")]
		public string? ModifiedDate { get; set; }
		[Display(Name = "點讚數")]
		public int FArticleLikes { get; set; }
		[Display(Name = "點閱數")]
		public int FArticleClicks { get; set; }
		[Display(Name = "內容")]
		public string? FArticleContent { get; set; }
		[Display(Name = "內容")]
		public string? PartialContent { get; set; }
		public bool FDeleted { get; set; }
		public bool LikeOrUnlike { get; set; }
	}
	public static class ArticleSearchVmExtensions
	{
		public static ArticleSearchVm SearchDtoToVm(this ArticleSearchDto dto)
		{
			string partialContent = Regex.Replace(dto.FArticleContent, "<.*?>", string.Empty).Trim();
			int len = Math.Min(partialContent.Length, 10);
			return new ArticleSearchVm()
			{
				FArticleId = dto.FArticleId,
				FArticleTitle = dto.FArticleTitle,
				FMemberPK = dto.FMemberId,
				PostingDate = dto.FArticlePostingDate.ToString("yyyy-MM-dd HH:mm:ss"),
				ModifiedDate = dto.FArticleModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"),
				FArticleLikes = dto.FArticleLikes,
				FArticleClicks = dto.FArticleClicks,
				FArticleContent = dto.FArticleContent,
				PartialContent = partialContent.Substring(0, len) + "...",
				FDeleted = dto.FDeleted,
			};
		}
	}
}
