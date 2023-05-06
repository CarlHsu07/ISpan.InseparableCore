using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleUpdateVm
	{
		public int FArticleId { get; set; }
		[Display(Name = "會員Id")]
		public int FMemberId { get; set; }
		[Display(Name = "標題")]
		[Required(ErrorMessage = "必填")]
		public string? FArticleTitle { get; set; }
		[Display(Name = "類別")]
		[Required(ErrorMessage = "必填")]
		public int FArticleCategoryId { get; set; }
		[Display(Name = "內容")]
		[Required(ErrorMessage = "必填")]
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
