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

		public IEnumerable<ArticleVm> Search(ArticleSearchCondition? condition)
		{
			var articles = context.TArticles.Where(t => t.FDeleted == false).Include(t => t.FMember)
				.OrderByDescending(t => t.FArticlePostingDate).ToList();

			if (condition == null) return ModelToVms(articles);
			//id搜尋
			if (int.TryParse(condition.Key, out int articleId))
			{
				articles = articles.Where(t => t.FArticleId == articleId).ToList();
				return ModelToVms(articles);
			}


			//關鍵字key
			if (!string.IsNullOrEmpty(condition.Key))
			{
				articles = articles.Where(t => t.FArticleTitle.Contains(condition.Key) 
											|| t.FArticleContent.Contains(condition.Key)
											|| (t.FMember.FLastName + t.FMember.FFirstName).Contains(condition.Key)).ToList();
			}
			//電影類別
			if (condition.CategoryId.HasValue && condition.CategoryId != 0)
			{
				articles = articles.Where(t => t.FArticleCategoryId == condition.CategoryId).ToList();
			}

			return ModelToVms(articles);
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
		public async Task CreateAsync(ArticleVm vm)
		{
			//新增Article
			vm.FArticlePostingDate = DateTime.Now;
			vm.FArticleModifiedDate = DateTime.Now;
			TArticles article = vm.VmToModel();

			context.Add(article);
			context.SaveChanges();

			await context.SaveChangesAsync();
		}
		public async Task UpdateAsync(ArticleVm vm)
		{
			TArticles article = context.TArticles.Find(vm.FArticleId);
			article.FArticleModifiedDate = DateTime.Now;
			article.FArticleTitle = vm.FArticleTitle;
			article.FArticleCategoryId = vm.FArticleCategoryId;
			article.FArticleContent = vm.FArticleContent;
			article.FArticleLikes = vm.FArticleLikes;

			context.Update(article);
			await context.SaveChangesAsync();

		}

		public async Task UpdateLikeAsync(ArticleVm vm)
		{
			TArticles article = context.TArticles.Find(vm.FArticleId);
			article.FArticleLikes = vm.FArticleLikes;

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
			}
		}

		public IEnumerable<TArticles> Articles (string keyword)
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
											.Where(t=>t.FDeleted==false).ToList();
            }

			return articles;
        }
	}
}
