using ISpan.InseparableCore.Models.BLL.Cores;

namespace ISpan.InseparableCore.Models.BLL.DTOs
{
	public class ArticleCreateDto
	{
		public string FArticleTitle { get; set; }
		public int FMemberId { get; set; }
		public int FArticleCategoryId { get; set; }
		public string FArticleContent { get; set; }
	}
	public static class ArticleCreateDtoExtensions
	{
		public static ArticleEntity CreateDtoToEntity(this ArticleCreateDto dto)
		{
			return new ArticleEntity()
			{
				FArticleTitle = dto.FArticleTitle,
				FMemberId = dto.FMemberId,
				FArticleContent = dto.FArticleContent,
				FArticleCategoryId = dto.FArticleCategoryId,
			};
		}

	}

}
