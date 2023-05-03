using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;

namespace ISpan.InseparableCore.Models.BLL
{
	public class MovieService
	{
		private readonly MovieRepository repo;

		public MovieService(MovieRepository repo)
		{
			this.repo = repo;
		}
		//public IEnumerable<MovieSearchDto> Search(MovieSearchCondition? condition)
		//{
		//	IEnumerable<MovieEntity> entities = repo.Search(condition);

		//	return entities.SearchEntitiesToDtos();
		//}

		public void Create(MovieCreateDto dto)
		{
			var entity = dto.CreateDtoToEntity();

			//if (string.IsNullOrEmpty(entity.Title) || string.IsNullOrEmpty(entity.Content)) throw new Exception("請正確填寫標題及內容");

			//// 驗證Title 是否唯一
			//var entityInDbByTitle = repo.GetByTitle(entity.Title);
			//if (entityInDbByTitle != null) throw new Exception("此標題已被使用");

			repo.Create(entity);
		}

		public void Update(MovieUpdateDto dto)
		{
			var currentMovie = repo.GetByMovieId(dto.FMovieId);

			var updateEntity = dto.UpdateDtoToEntity();

			//if (string.IsNullOrEmpty(updateEntity.Title) || string.IsNullOrEmpty(updateEntity.Content)) throw new Exception("請正確填寫標題及內容");

			//// 驗證Title 是否唯一
			//var entityInDb = repo.GetByTitle(updateEntity.Title);
			//if (entityInDb != null && entityInDb.MovieID != updateEntity.MovieID) throw new Exception("此標題已被使用");

			repo.Update(updateEntity);
		}
		public MovieUpdateDto GetUpdateDto(int movieId)
		{
			var entity = repo.GetByMovieId(movieId);
			return entity.UpdateEntityToDto();
		}

		public MovieSearchDto GetSearchDto(int movieId)
		{
			var entity = repo.GetByMovieId(movieId);
			return entity.SearchEntityToDto();
		}
	}
}
