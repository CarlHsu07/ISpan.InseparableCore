using ISpan.InseparableCore.Models;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieCreateVm
	{
		public int FMovieId { get; set; }
		public string? FMovieName { get; set; }
		public string? FMovieIntroduction { get; set; }
		public int FMovieLevelId { get; set; }
		public DateTime FMovieOnDate { get; set; }
		public DateTime? FMovieOffDate { get; set; }
		public int FMovieLength { get; set; }
		public string? FMovieImagePath { get; set; }
		public int FMovieScore { get; set; }
		public string? CategoryIds { get; set; }
		public List<TMovieCategoryDetails>? CategoryDetails { get; set; }
		public IFormFile? Image { get; set; }
	}
	public static class MovieVmExtensions
	{
		public static MovieCreateVm ModelToVm(this TMovies tMovies)
		{
			MovieCreateVm vm = new MovieCreateVm()
			{
				FMovieId = tMovies.FMovieId,
				FMovieIntroduction = tMovies.FMovieIntroduction,
				FMovieName = tMovies.FMovieName,
				FMovieLevelId = tMovies.FMovieLevelId,
				FMovieOnDate = tMovies.FMovieOnDate,
				FMovieOffDate = tMovies.FMovieOffDate,
				FMovieLength = tMovies.FMovieLength,
				FMovieScore = tMovies.FMovieScore,
				FMovieImagePath = tMovies.FMovieImagePath,
			};
			vm.CategoryDetails = (new InseparableContext()).TMovieCategoryDetails.Where(t => t.FMovieId == tMovies.FMovieId).ToList();
			return vm;
		}
	}
}
