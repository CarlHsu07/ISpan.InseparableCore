using Humanizer;
using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.EntityFrameworkCore;

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

			// 驗證Name 是否唯一
			var entityInDb = repo.GetbyMovieName(entity.FMovieName);
			if (entityInDb != null && entityInDb.FMovieId != entity.FMovieId) throw new Exception("此名稱已被使用");

			repo.Create(entity);
		}

		public void Update(MovieUpdateDto dto)
		{
			var currentMovie = repo.GetByMovieId(dto.FMovieId);

			var updateEntity = dto.UpdateDtoToEntity();
			//此處linq語法可用bool輸出
			if (repo.GetByMovieId(dto.FMovieId) == null) throw new Exception("此電影不存在");

			// 驗證Name 是否唯一
			var entityInDb = repo.GetbyMovieName(updateEntity.FMovieName);
			if (entityInDb != null && entityInDb.FMovieId != updateEntity.FMovieId) throw new Exception("此名稱已被使用");

			repo.Update(updateEntity);
		}
		public MovieUpdateDto GetUpdateDto(int movieId)
		{
			var entity = repo.GetByMovieId(movieId);
			if (entity == null) throw new Exception("此電影不存在");

			return entity.UpdateEntityToDto();
		}

		public MovieSearchDto GetSearchDto(int movieId)
		{
			var entity = repo.GetByMovieId(movieId);
			if (entity == null) throw new Exception("此電影不存在");

			return entity.SearchEntityToDto();
		}

		public async Task Delete(int movieId)
		{
			var movie = repo.GetByMovieId(movieId);
			if (movie == null) throw new Exception("此電影不存在");

			await repo.Delete(movieId);
		}
	}
}
