using ISpan.InseparableCore.Models.BLL.Cores;

namespace ISpan.InseparableCore.Models.BLL.DTOs
{
	public class MovieCreateDto
	{
		public string FMovieName { get; set; }
		public string FMovieIntroduction { get; set; }
		public int FMovieLevelId { get; set; }
		public DateTime FMovieOnDate { get; set; }
		public DateTime? FMovieOffDate { get; set; }
		public int FMovieLength { get; set; }
		public string FMovieImagePath { get; set; }
		public string FMovieActors { get; set; }
		public string FMovieDirectors { get; set; }
		public MovieEntity CreateDtoToEntity(MovieCreateDto dto)
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
