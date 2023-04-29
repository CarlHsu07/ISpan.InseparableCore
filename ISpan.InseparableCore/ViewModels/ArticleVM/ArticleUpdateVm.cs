using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleUpdateVm
	{
		public int FArticleId { get; set; }
		public int FMemberId { get; set; }
		[DisplayName("標題")]
		public string? FArticleTitle { get; set; }
		[DisplayName("類別")]
		public int FArticleCategoryId { get; set; }
		[DisplayName("內容")]
		public string? FArticleContent { get; set; }
	}
	public static class ArticleUpdateVmExtensions
	{
		public static ArticleUpdateVm UpdateDtoToVm(this ArticleUpdateDto dto)
		{
			return new ArticleUpdateVm
			{
				FArticleId = dto.FArticleId,
				FMemberId = dto.FMemberId,
				FArticleTitle = dto.FArticleTitle,
				FArticleContent = dto.FArticleContent,
				FArticleCategoryId = dto.FArticleCategoryId,
			};
		}

		public static ArticleUpdateDto UpdateVmToDto(this ArticleUpdateVm vm)
		{
			return new ArticleUpdateDto
			{
				FArticleId = vm.FArticleId,
				FArticleTitle = vm.FArticleTitle,
				FArticleContent = vm.FArticleContent,
				FArticleCategoryId = vm.FArticleCategoryId,
			};
		}
	}
}
