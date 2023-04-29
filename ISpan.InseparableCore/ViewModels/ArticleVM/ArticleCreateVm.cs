using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleCreateVm
	{
		[DisplayName("標題")]
		public string? FArticleTitle { get; set; }
		[DisplayName("發文者")]
		public int? FMemberId { get; set; }
		[DisplayName("類別")]
		public int FArticleCategoryId { get; set; }
		[DisplayName("內容")]
		public string? FArticleContent { get; set; }
	}
	public static class ArticleCreateVmExtensions
	{
		public static ArticleCreateDto CreateVmToDto(this ArticleCreateVm vm)
		{
			return new ArticleCreateDto()
			{
				FArticleTitle = vm.FArticleTitle,
				FMemberId = (int)vm.FMemberId,
				FArticleContent = vm.FArticleContent,
				FArticleCategoryId = vm.FArticleCategoryId,
			};
		}

	}
}
