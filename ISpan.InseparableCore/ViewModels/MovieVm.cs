using ISpan.InseparableCore.Models;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieVm
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
		public List<TMovieActorDetails>? MovieActorDetails { get; set; }
		public List<TMovieDirectorDetails>? MovieDirectorDetails { get; set; }
		public IFormFile? Image { get; set; }
	}
	public static class MovieVmExtensions
	{
		public static MovieVm ModelToVm(this TMovies tMovies)
		{
			MovieVm vm = new MovieVm()
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
			vm.MovieActorDetails = (new InseparableContext()).TMovieActorDetails.Where(t => t.FMovieId == tMovies.FMovieId).ToList();
			vm.MovieDirectorDetails = (new InseparableContext()).TMovieDirectorDetails.Where(t => t.FMovieId == tMovies.FMovieId).ToList();
			return vm;
		}
		public static TMovies VmToModel(this MovieVm vm)
		{
			var movie = new TMovies()
			{
				FMovieId = vm.FMovieId,
				FMovieIntroduction = vm.FMovieIntroduction,
				FMovieName = vm.FMovieName,
				FMovieLevelId = vm.FMovieLevelId,
				FMovieOnDate = vm.FMovieOnDate,
				FMovieOffDate = vm.FMovieOffDate,
				FMovieLength = vm.FMovieLength,
				FMovieScore = vm.FMovieScore,
				FMovieImagePath = vm.FMovieImagePath,
			};
			return movie;
		}
	}
}
