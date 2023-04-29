using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleSearchVm
	{
		public int FArticleId { get; set; }
		[DisplayName("標題")]
		public string? FArticleTitle { get; set; }
		[DisplayName("發文者")]
		public int? FMemberPK { get; set; }
		[DisplayName("發文ID")]
		public string? FMemberId { get; set; }
		[DisplayName("發文者")]
		public string? MemberName { get; set; }
		[DisplayName("類別")]
		public string? ArticleCategory { get; set; }
		[DisplayName("發文時間")]
		public string? PostingDate { get; set; }
		[DisplayName("修改時間")]
		public string? ModifiedDate { get; set; }
		[DisplayName("點讚數")]
		public int FArticleLikes { get; set; }
		[DisplayName("點閱數")]
		public int FArticleClicks { get; set; }
		[DisplayName("內容")]
		public string? FArticleContent { get; set; }
		[DisplayName("內容")]
		public string? PartialContent { get; set; }
		public bool FDeleted { get; set; }
		public bool LikeOrUnlike { get; set; }
	}
	public static class ArticleSearchVmExtensions
	{
		public static ArticleSearchVm SearchDtoToVm(this ArticleSearchDto dto)
		{
			int len = Math.Min(dto.FArticleContent.Length, 10);
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
				PartialContent = dto.FArticleContent.Trim().Substring(0, len) + "...",
				FDeleted = dto.FDeleted,
			};
		}
	}
}
