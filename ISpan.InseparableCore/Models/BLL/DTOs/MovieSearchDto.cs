using ISpan.InseparableCore.Models.BLL.Cores;

namespace ISpan.InseparableCore.Models.BLL.DTOs
{
	public class MovieSearchDto
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
	public static class MovieSearchDtoExtensions
	{
		public static MovieSearchDto SearchEntityToDto(this MovieEntity entity)
		{
			return new MovieSearchDto
			{
				FMovieId = entity.FMovieId,
				FMovieName = entity.FMovieName,
				FMovieIntroduction = entity.FMovieIntroduction,
				FMovieLevelId = entity.FMovieLevelId,
				FMovieOnDate = entity.FMovieOnDate,
				FMovieOffDate = entity.FMovieOffDate,
				FMovieLength = entity.FMovieLength,
				FMovieImagePath = entity.FMovieImagePath,
				FMovieActors = entity.FMovieActors,
				FMovieDirectors = entity.FMovieDirectors,
				FMovieScore = entity.FMovieScore,
			};
		}
		public static IEnumerable<MovieSearchDto> SearchEntitiesToDtos(this IEnumerable<MovieEntity> entities)
		{
			var dtos = new List<MovieSearchDto>();

			foreach (var entity in entities)
			{
				MovieSearchDto dto = entity.SearchEntityToDto();
				dtos.Add(dto);
			}
			return dtos;
		}
	}
}
