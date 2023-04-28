using Humanizer;
using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ISpan.InseparableCore.Models.BLL.Cores
{
	public class MovieEntity
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
		//public MovieEntity(int fMovieId, string fMovieName, string fMovieIntroduction, int fMovieLevelId, DateTime fMovieOnDate, DateTime? fMovieOffDate, int fMovieLength, string fMovieImagePath, decimal fMovieScore, string fMovieActors, string fMovieDirectors, bool fDeleted)
		//{
		//	FMovieId = fMovieId;
		//	FMovieName = fMovieName;
		//	FMovieIntroduction = fMovieIntroduction;
		//	FMovieLevelId = fMovieLevelId;
		//	FMovieOnDate = fMovieOnDate;
		//	FMovieOffDate = fMovieOffDate;
		//	FMovieLength = fMovieLength;
		//	FMovieImagePath = fMovieImagePath;
		//	FMovieScore = fMovieScore;
		//	FMovieActors = fMovieActors;
		//	FMovieDirectors = fMovieDirectors;
		//	FDeleted = fDeleted;
		//}		
	}
	public static class MovieEntityExtensions
	{
		public static MovieEntity ModelToEntity(this TMovies movie)
		{
			return new MovieEntity
			{
				FMovieId = movie.FMovieId,
				FMovieName = movie.FMovieName,
				FMovieIntroduction = movie.FMovieIntroduction,
				FMovieLevelId = movie.FMovieLevelId,
				FMovieOnDate = movie.FMovieOnDate,
				FMovieOffDate = movie.FMovieOffDate,
				FMovieLength = movie.FMovieLength,
				FMovieImagePath = movie.FMovieImagePath,
				FMovieScore = movie.FMovieScore,
				FMovieActors = movie.FMovieActors,
				FMovieDirectors = movie.FMovieDirectors,
				FDeleted = movie.FDeleted,
			};
		}
		public static IEnumerable<MovieEntity> ModelsToEntities(this IEnumerable<TMovies> movies)
		{
			var entities = new List<MovieEntity>();

			foreach (var movie in movies)
			{
				MovieEntity entity = movie.ModelToEntity();
				entities.Add(entity);
			}
			return entities;
		}

	}
}
