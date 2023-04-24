using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.Build.Framework;
using System.ComponentModel;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieVm
	{
		[DisplayName("電影ID")]
		public int FMovieId { get; set; }
		[DisplayName("名稱")]
		[Required]
		public string? FMovieName { get; set; }
		[Required]
		[DisplayName("簡介")]
		public string? FMovieIntroduction { get; set; }
		[DisplayName("簡介")]
		public string? PartialIntro { get; set; }
		[Required]
		[DisplayName("電影分級")]
		public int FMovieLevelId { get; set; }
		[DisplayName("電影分級")]
		[Required]
		public string? Level { get; set; }
		[DisplayName("上映日期")]
		[Required]
		public DateTime FMovieOnDate { get; set; }
		[DisplayName("上映日期")]
		[Required]
		public string? OnDate { get; set; }
		[DisplayName("下映日期")]
		public DateTime? FMovieOffDate { get; set; }
		[DisplayName("下映日期")]
		public string? OffDate { get; set; }
		[Required]
		[DisplayName("片長")]
		public int FMovieLength { get; set; }
		[DisplayName("圖片檔路徑")]
		public string? FMovieImagePath { get; set; }
		[DisplayName("會員評分")]
		public decimal? FMovieScore { get; set; } = 0;
		[DisplayName("電影類別")]
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
		public static MovieVm ModelToVm(this TMovies movie)
		{
			int len = Math.Min(movie.FMovieIntroduction.Length, 10);

			MovieVm vm = new MovieVm()
			{
				FMovieId = movie.FMovieId,
				FMovieIntroduction = movie.FMovieIntroduction,
				PartialIntro = movie.FMovieIntroduction.Trim().Substring(0, len) + "...",
				FMovieName = movie.FMovieName,
				FMovieLevelId = movie.FMovieLevelId,
				FMovieOnDate = movie.FMovieOnDate,
				OnDate = movie.FMovieOnDate.ToString("yyyy-MM-dd"),
				FMovieOffDate = movie.FMovieOffDate,
				OffDate = ((DateTime)movie.FMovieOffDate).ToString("yyyy-MM-dd"),
				FMovieLength = movie.FMovieLength,
				FMovieScore = movie.FMovieScore,
				FMovieImagePath = movie.FMovieImagePath,
				FMovieActors = movie.FMovieActors,
				FMovieDirectors= movie.FMovieDirectors,
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
