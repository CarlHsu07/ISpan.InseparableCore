using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ISpan.InseparableCore.Models.DAL
{
	public class ArticleRepository
	{
		private readonly InseparableContext context;

		public ArticleRepository(InseparableContext context)
		{
			this.context = context;
		}

		public IEnumerable<ArticleEntity> Search(ArticleSearchCondition? condition)
		{
			IEnumerable<TArticles> articles = context.TArticles.Where(t => t.FDeleted == false)
				.OrderByDescending(t => t.FArticlePostingDate);

			if (condition == null) return articles.ModelsToEntities();
			//id搜尋
			if (int.TryParse(condition.Key, out int articleId))
			{
				articles = articles.Where(t => t.FArticleId == articleId);
				return articles.ModelsToEntities();
			}

			//關鍵字key
			if (!string.IsNullOrEmpty(condition.Key))
			{
				articles = articles.Where(t => t.FArticleTitle.Contains(condition.Key)
											|| t.FArticleContent.Contains(condition.Key)
											|| (t.FMember.FLastName + t.FMember.FFirstName).Contains(condition.Key));
			}
			//電影類別
			if (condition.CategoryId.HasValue && condition.CategoryId != 0)
			{
				articles = articles.Where(t => t.FArticleCategoryId == condition.CategoryId);
			}

			return articles.ModelsToEntities();
		}
		public ArticleEntity GetByArticleId(int articleId)
		{
			TArticles article = context.TArticles.Find(articleId);
			if (article == null) return null;

			return article.ModelToEntity();
		}

		public async Task Create(ArticleEntity entity)
		{
			TArticles article = new TArticles();

			article.FArticleTitle = entity.FArticleTitle;
			article.FMemberId = entity.FMemberId;
			article.FArticleCategoryId = entity.FArticleCategoryId;
			article.FArticleContent = entity.FArticleContent;
			article.FArticleClicks = 0;

			article.FArticleLikes = 0;
			article.FDeleted = false;
			article.FArticlePostingDate = DateTime.Now;
			article.FArticleModifiedDate = DateTime.Now;

			context.Add(article);
			await context.SaveChangesAsync();
		}
		public async Task Update(ArticleEntity entity)
		{
			TArticles article = context.TArticles.Find(entity.FArticleId);

			article.FArticleTitle = entity.FArticleTitle;
			article.FArticleCategoryId = entity.FArticleCategoryId;
			article.FArticleContent = entity.FArticleContent;
			article.FArticleModifiedDate = DateTime.Now;

			context.Update(article);
			await context.SaveChangesAsync();
		}

		public async Task UpdateLikes(int articleId, int likes)
		{
			TArticles article = context.TArticles.Find(articleId);
			article.FArticleLikes = likes;

			context.Update(article);
			await context.SaveChangesAsync();
		}
		public void Click(int articleId)
		{
			TArticles article = context.TArticles.Find(articleId);
			article.FArticleClicks++;

			context.Update(article);
			context.SaveChanges();
		}
		public void Delete(int articleId)
		{
			var article = context.TArticles.Find(articleId);
			if (article != null)
			{
				article.FDeleted = true;
				context.TArticles.Update(article);
				context.SaveChanges();
			}
		}
		public IEnumerable<TArticles> Articles(string keyword)
		{
			if (keyword == null)
				return null;

			List<TArticles> articles = null;
			if (!string.IsNullOrEmpty(keyword))
			{
				articles = context.TArticles.Where(t => t.FArticleTitle.Contains(keyword)
													|| t.FArticleContent.Contains(keyword)
													|| (t.FMember.FFirstName).Contains(keyword)
													|| (t.FMember.FLastName).Contains(keyword))
											.Where(t => t.FDeleted == false).ToList();
			}

			return articles;
		}

		public string GetCategory(int categoryId)
		{
			return context.TMovieCategories.Find(categoryId).FMovieCategoryName;
		}

		public TMembers GetMemberByPK(int pk)
		{
			return context.TMembers.Find(pk);
		}
	}
}
