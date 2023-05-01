using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleCreateVm
	{
		[Display(Name = "標題")]
		[Required(ErrorMessage = "必填")]
		public string? FArticleTitle { get; set; }
		[Display(Name = "發文者")]
		[Required(ErrorMessage = "必填")]
		public int FMemberId { get; set; }
		[Display(Name = "類別")]
		[Required(ErrorMessage = "必填")]
		public int FArticleCategoryId { get; set; }
		[Display(Name = "內容")]
		[Required(ErrorMessage = "必填")]
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
