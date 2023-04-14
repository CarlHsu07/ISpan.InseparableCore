using ISpan.InseparableCore.Models;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System.ComponentModel;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieVm
	{
		[DisplayName("電影ID")]
		public int FMovieId { get; set; }
		[DisplayName("名稱")]
		public string? FMovieName { get; set; }
		[DisplayName("簡介")]
		public string? FMovieIntroduction { get; set; }
		[DisplayName("電影分級")]
		public int FMovieLevelId { get; set; }
		[DisplayName("電影分級")]
		public string? Level { get; set; }
		[DisplayName("上映日期")]
		public DateTime FMovieOnDate { get; set; }

		[DisplayName("上映日期")]
		public string? OnDate { get; set; }
		[DisplayName("下映日期")]
		public DateTime? FMovieOffDate { get; set; }
		[DisplayName("片長")]
		public int FMovieLength { get; set; }
		[DisplayName("圖片檔路徑")]
		public string? FMovieImagePath { get; set; }
		[DisplayName("會員評分")]
		public int? FMovieScore { get; set; } = 0;
		[DisplayName("電影類別")]
		public string? CategoryIds { get; set; }

		[DisplayName("電影類別")]
		public string? Categories { get; set; }
		public List<TMovieActorDetails>? MovieActorDetails { get; set; }
		public List<TMovieDirectorDetails>? MovieDirectorDetails { get; set; }
		[DisplayName("宣傳照")]
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
				OnDate = tMovies.FMovieOnDate.ToString("yyyy-MM-dd"),
				FMovieOffDate = tMovies.FMovieOffDate,
				FMovieLength = tMovies.FMovieLength,
				FMovieScore = tMovies.FMovieScore,
				FMovieImagePath = tMovies.FMovieImagePath,
			};
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
				FMovieOffDate = vm.FMovieOnDate.AddMonths(1),
				FMovieLength = vm.FMovieLength,
				FMovieScore = (int)vm.FMovieScore,
				FMovieImagePath = vm.FMovieImagePath,
			};
			return movie;
		}
	}
}
