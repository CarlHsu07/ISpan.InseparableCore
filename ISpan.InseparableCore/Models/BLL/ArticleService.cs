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

			repo.Create(entity);
		}

		public void Update(ArticleUpdateDto dto)
		{
			var currentArticle = repo.GetByArticleId(dto.FArticleId);

			var updateEntity = dto.UpdateDtoToEntity();

			//if (string.IsNullOrEmpty(updateEntity.Title) || string.IsNullOrEmpty(updateEntity.Content)) throw new Exception("請正確填寫標題及內容");

			//// 驗證Title 是否唯一
			//var entityInDb = repo.GetByTitle(updateEntity.Title);
			//if (entityInDb != null && entityInDb.ArticleID != updateEntity.ArticleID) throw new Exception("此標題已被使用");

			repo.Update(updateEntity);
		}
		public ArticleUpdateDto GetUpdateDto(int movieId)
		{
			var entity = repo.GetByArticleId(movieId);
			return entity.UpdateEntityToDto();
		}

		public ArticleSearchDto GetSearchDto(int movieId)
		{
			var entity = repo.GetByArticleId(movieId);
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
