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
			var articles = context.TArticles.ToList();

			if (condition == null) return ModelToVms(articles);

			//關鍵字key
			if (!string.IsNullOrEmpty(condition.Key))
			{
				articles = articles.Where(t => t.FArticleTitle.Contains(condition.Key) 
											|| t.FArticleContent.Contains(condition.Key)).ToList();
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
				ArticleVm vm = article.ModelToVm();
				vm.ArticleCategory = context.TArticles.Include(t => t.FArticleCategory)
					.FirstOrDefault(t => t.FArticleId == vm.FArticleId).FArticleCategory.FMovieCategoryName;
				vm.MemberName = context.TArticles.Include(t => t.FMember)
					.FirstOrDefault(t => t.FArticleId == vm.FArticleId).FMember.FFirstName;

				vms.Add(vm);
			}
			return vms;
		}

		public ArticleVm GetVmById(int id)
		{
			TArticles article = context.TArticles.FirstOrDefault(t => t.FArticleId == id);

			ArticleVm vm = article.ModelToVm();
			vm.ArticleCategory = context.TArticles.Include(t => t.FArticleCategory)
	.FirstOrDefault(t => t.FArticleId == vm.FArticleId).FArticleCategory.FMovieCategoryName;
			vm.MemberName = context.TArticles.Include(t => t.FMember)
				.FirstOrDefault(t => t.FArticleId == vm.FArticleId).FMember.FFirstName;
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
			vm.FArticleModifiedDate = DateTime.Now;
			TArticles article = vm.VmToModel();

			context.Update(article);
			await context.SaveChangesAsync();

		}
		public void Click(ArticleVm vm)
		{
			TArticles article = vm.VmToModel();
			article.FArticleClicks++;
			context.Update(article);
			context.SaveChanges();
		}
		public async Task DeleteAsync(int articleId)
		{
			var article = await context.TArticles.FindAsync(articleId);
			if (article != null)
			{
				context.TArticles.Remove(article);
			}
		}

	}
}
