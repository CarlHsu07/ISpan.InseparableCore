using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.DAL;
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
		public DateTime FMovieOnDate { get; set; }
		[DisplayName("上映日期")]
		public string? OnDate { get; set; }
		public DateTime? FMovieOffDate { get; set; }
		[DisplayName("下映日期")]
		public string? OffDate { get; set; }
		[DisplayName("片長")]
		public int FMovieLength { get; set; }
		[DisplayName("圖片檔路徑")]
		public string? FMovieImagePath { get; set; }
		[DisplayName("會員評分")]
		public int? FMovieScore { get; set; } = 0;
		public string? CategoryIds { get; set; }

		[DisplayName("電影類別")]
		public string? Categories { get; set; }
		[DisplayName("主要演員")]
		public string? FMovieActors { get; set; }
		[DisplayName("導演")]
		public string? FMovieDirectors { get; set; }
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
				OffDate = ((DateTime)tMovies.FMovieOffDate).ToString("yyyy-MM-dd"),
				FMovieLength = tMovies.FMovieLength,
				FMovieScore = tMovies.FMovieScore,
				FMovieImagePath = tMovies.FMovieImagePath,
				FMovieActors = tMovies.FMovieActors,
				FMovieDirectors= tMovies.FMovieDirectors,
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
				FMovieActors = vm.FMovieActors,
				FMovieDirectors= vm.FMovieDirectors,
			};
			return movie;
		}
	}
}
