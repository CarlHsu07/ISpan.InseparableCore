using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.Models.BLL.Cores
{
	public class ArticleEntity
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
	public static class ArticleEntityExtensions
	{
		public static ArticleEntity ModelToEntity(this TArticles article)
		{
			return new ArticleEntity
			{
				FArticleId = article.FArticleId,
				FArticleTitle = article.FArticleTitle,
				FMemberId = article.FMemberId,
				FArticleCategoryId = article.FArticleCategoryId,
				FArticlePostingDate = article.FArticlePostingDate,
				FArticleModifiedDate = article.FArticleModifiedDate,
				FArticleClicks = article.FArticleClicks,
				FArticleLikes = article.FArticleLikes,
				FArticleContent = article.FArticleContent,
				FDeleted = article.FDeleted,
			};
		}
		public static IEnumerable<ArticleEntity> ModelsToEntities(this IEnumerable<TArticles> articles)
		{
			var entities = new List<ArticleEntity>();

			foreach (var movie in articles)
			{
				ArticleEntity entity = movie.ModelToEntity();
				entities.Add(entity);
			}
			return entities;
		}
	}

}
