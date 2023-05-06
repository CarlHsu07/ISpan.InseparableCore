using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;

namespace ISpan.InseparableCore.Models.BLL
{
	public class ArticleService
	{
		private readonly ArticleRepository repo;

		public ArticleService(ArticleRepository repo)
		{
			this.repo = repo;
		}
		public IEnumerable<ArticleSearchDto> Search(ArticleSearchCondition? condition)
		{
			IEnumerable<ArticleEntity> entities = repo.Search(condition);

			return entities.SearchEntitiesToDtos();
		}

		public void Create(ArticleCreateDto dto)
		{
			var entity = dto.CreateDtoToEntity();
			if (string.IsNullOrEmpty(entity.FArticleTitle) || string.IsNullOrEmpty(entity.FArticleContent)) throw new Exception("請正確填寫標題及內容");

			var entityInDb = repo.GetByTitle(entity.FArticleTitle);
			if (entityInDb != null) throw new Exception("此標題已被使用");

			repo.Create(entity);
		}

		public void Update(ArticleUpdateDto dto)
		{
			var currentArticle = repo.GetByArticleId(dto.FArticleId);
			if (currentArticle == null) throw new Exception("此文章不存在");

			var updateEntity = dto.UpdateDtoToEntity();

			if (string.IsNullOrEmpty(updateEntity.FArticleTitle) || string.IsNullOrEmpty(updateEntity.FArticleContent)) throw new Exception("請正確填寫標題及內容");

			// 驗證Title 是否唯一
			var entityInDb = repo.GetByTitle(updateEntity.FArticleTitle);
			if (entityInDb != null && entityInDb.FArticleId != updateEntity.FArticleId) throw new Exception("此標題已被使用");

			repo.Update(updateEntity);
		}
		public ArticleUpdateDto GetUpdateDto(int movieId)
		{
			var entity = repo.GetByArticleId(movieId);
			if (entity == null) throw new Exception("此文章不存在");

			return entity.UpdateEntityToDto();
		}

		public ArticleSearchDto GetSearchDto(int movieId)
		{
			var entity = repo.GetByArticleId(movieId);
			if (entity == null) throw new Exception("此文章不存在");

			return entity.SearchEntityToDto();
		}

		public int getArticleLikes(int articleId)
		{
			return repo.GetByArticleId(articleId).FArticleLikes;
		}

		public void Click(int articleId)
		{
			repo.Click(articleId);
		}
	}
}
