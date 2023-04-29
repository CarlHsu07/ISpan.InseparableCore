using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.ViewModels;

namespace ISpan.InseparableCore.Models.BLL.DTOs
{
	public class ArticleSearchDto
	{
		public int FArticleId { get; set; }
		public string FArticleTitle { get; set; }
		public int FMemberId { get; set; }
		public int FArticleCategoryId { get; set; }
		public DateTime FArticlePostingDate { get; set; }
		public DateTime FArticleModifiedDate { get; set; }
		public int FArticleLikes { get; set; }
		public int FArticleClicks { get; set; }
		public string FArticleContent { get; set; }
		public bool FDeleted { get; set; }
	}
	public static class ArticleSearchDtoExtensions
	{
		public static ArticleSearchDto SearchEntityToDto(this ArticleEntity entity)
		{
			return new ArticleSearchDto()
			{
				FArticleId = entity.FArticleId,
				FArticleTitle = entity.FArticleTitle,
				FMemberId = entity.FMemberId,
				FArticleCategoryId = entity.FArticleCategoryId,
				FArticlePostingDate = entity.FArticlePostingDate,
				FArticleModifiedDate = entity.FArticleModifiedDate,
				FArticleLikes = entity.FArticleLikes,
				FArticleClicks = entity.FArticleClicks,
				FArticleContent = entity.FArticleContent,
				FDeleted = entity.FDeleted,
			};
		}
		public static IEnumerable<ArticleSearchDto> SearchEntitiesToDtos(this IEnumerable<ArticleEntity> entities)
		{
			var dtos = new List<ArticleSearchDto>();

			foreach (var entity in entities)
			{
				ArticleSearchDto dto = entity.SearchEntityToDto();
				dtos.Add(dto);
			}
			return dtos;
		}

	}
}
