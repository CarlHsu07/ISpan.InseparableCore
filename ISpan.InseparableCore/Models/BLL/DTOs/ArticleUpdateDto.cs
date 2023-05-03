using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.ViewModels;

namespace ISpan.InseparableCore.Models.BLL.DTOs
{
	public class ArticleUpdateDto
	{
		public int FArticleId { get; set; }
		public int FMemberId { get; set; }
		public string FArticleTitle { get; set; }
		public int FArticleCategoryId { get; set; }
		public string FArticleContent { get; set; }
	}
	public static class ArticleUpdateDtoExtensions
	{
		public static ArticleUpdateDto UpdateEntityToDto(this ArticleEntity dto)
		{
			return new ArticleUpdateDto()
			{
				FArticleId = dto.FArticleId,
				FMemberId = dto.FMemberId,
				FArticleTitle = dto.FArticleTitle,
				FArticleCategoryId = dto.FArticleCategoryId,
				FArticleContent = dto.FArticleContent,
			};
		}
		public static ArticleEntity UpdateDtoToEntity(this ArticleUpdateDto dto)
		{
			return new ArticleEntity()
			{
				FArticleId = dto.FArticleId,
				FArticleTitle = dto.FArticleTitle,
				FArticleContent = dto.FArticleContent,
				FArticleCategoryId = dto.FArticleCategoryId,
			};
		}

	}

}
