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
				.Include(t => t.FMember).OrderByDescending(t => t.FArticlePostingDate);

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
		public ArticleEntity GetByTitle(string title)
		{
			var article = context.TArticles.FirstOrDefault(t => t.FArticleTitle.Equals(title) && !t.FDeleted);
			if (article == null) return null;
			return article.ModelToEntity();
		}
		public ArticleEntity GetByArticleId(int articleId)
		{
			TArticles article = context.TArticles.Find(articleId);
			if (article == null || article.FDeleted) return null;

			return article.ModelToEntity();
		}

		public void Create(ArticleEntity entity)
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
			context.SaveChanges();
		}
		public void Update(ArticleEntity entity)
		{
			TArticles article = context.TArticles.Find(entity.FArticleId);

			article.FArticleTitle = entity.FArticleTitle;
			article.FArticleCategoryId = entity.FArticleCategoryId;
			article.FArticleContent = entity.FArticleContent;
			article.FArticleModifiedDate = DateTime.Now;

			context.Update(article);
			context.SaveChanges();
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
		public async Task Delete(int articleId)
		{
			var article = await context.TArticles.FindAsync(articleId);
			if (article == null || article.FDeleted) throw new Exception("此文章不存在");
			article.FDeleted = true;
			article.FArticleClicks = 0;
			context.Update(article);
			await context.SaveChangesAsync();
		}
		public string GetCategory(int categoryId)
		{
			return context.TMovieCategories.Find(categoryId).FMovieCategoryName;
		}

		public TMembers GetMemberByPK(int pk)
		{
			return context.TMembers.Find(pk);
		}
		public ArticleVm GetVmById(int id)
		{
			TArticles article = context.TArticles.Include(t => t.FArticleCategory)
				.Include(t => t.FMember).FirstOrDefault(t => t.FArticleId == id);

			ArticleVm vm = article.ModelToVm();
			vm.ArticleCategory = article.FArticleCategory.FMovieCategoryName;
			vm.MemberName = article.FMember.FLastName + article.FMember.FFirstName;
			vm.FMemberId = article.FMember.FMemberId;
			return vm;
		}
		public IEnumerable<ArticleVm> ModelToVms(IEnumerable<TArticles> articles)
		{
			List<ArticleVm> vms = new List<ArticleVm>();
			foreach (var article in articles)
			{
				ArticleVm vm = GetVmById(article.FArticleId);

				vms.Add(vm);
			}
			return vms;
		}

		public IEnumerable<ArticleVm> Articles(string keyword)
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

			return ModelToVms(articles);
		}

	}
}
