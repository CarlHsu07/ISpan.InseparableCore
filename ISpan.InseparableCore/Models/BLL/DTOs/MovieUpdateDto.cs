using ISpan.InseparableCore.Models.BLL.Cores;

namespace ISpan.InseparableCore.Models.BLL.DTOs
{
	public class MovieUpdateDto
	{
		public int FMovieId { get; set; }
		public string FMovieName { get; set; }
		public string FMovieIntroduction { get; set; }
		public int FMovieLevelId { get; set; }
		public DateTime FMovieOnDate { get; set; }
		public DateTime? FMovieOffDate { get; set; }
		public int FMovieLength { get; set; }
		public string FMovieImagePath { get; set; }
		public decimal FMovieScore { get; set; }
		public string FMovieActors { get; set; }
		public string FMovieDirectors { get; set; }
		public bool FDeleted { get; set; }
	}
	public static class MovieUpdateDtoExtensions
	{
		public static MovieUpdateDto UpdateEntityToDto(this MovieEntity entity)
		{
			return new MovieUpdateDto
			{
				FMovieName = entity.FMovieName,
				FMovieIntroduction = entity.FMovieIntroduction,
				FMovieLevelId = entity.FMovieLevelId,
				FMovieOnDate = entity.FMovieOnDate,
				FMovieOffDate = entity.FMovieOffDate,
				FMovieLength = entity.FMovieLength,
				FMovieImagePath = entity.FMovieImagePath,
				FMovieActors = entity.FMovieActors,
				FMovieDirectors = entity.FMovieDirectors,
			};
		}
		public static MovieEntity UpdateDtoToEntity(this MovieUpdateDto dto)
		{
			return new MovieEntity
			{
				FMovieName = dto.FMovieName,
				FMovieIntroduction = dto.FMovieIntroduction,
				FMovieLevelId = dto.FMovieLevelId,
				FMovieOnDate = dto.FMovieOnDate,
				FMovieOffDate = dto.FMovieOffDate,
				FMovieLength = dto.FMovieLength,
				FMovieImagePath = dto.FMovieImagePath,
				FMovieActors = dto.FMovieActors,
				FMovieDirectors = dto.FMovieDirectors,
			};
		}

	}

}
