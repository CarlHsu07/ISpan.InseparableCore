using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieSearchVm
	{
		[Display(Name = "電影ID")]
		public int FMovieId { get; set; }
		[Display(Name = "名稱")]
		public string? FMovieName { get; set; }
		[Display(Name = "簡介")]
		public string? PartialIntro { get; set; }
		[Display(Name = "簡介")]
		public string? FMovieIntroduction { get; set; }
		[Display(Name = "電影分級")]
		public string? Level { get; set; }
		[Display(Name = "上映日期")]
		public string? OnDate { get; set; }
		[Display(Name = "下映日期")]
		public string? OffDate { get; set; }
		[Display(Name = "片長(分鐘)")]
		public int FMovieLength { get; set; }
		[Display(Name = "圖片檔路徑")]
		public string? FMovieImagePath { get; set; }
		[Display(Name = "會員評分")]
		public decimal? FMovieScore { get; set; }
		[Display(Name = "電影類別")]
		public string? Categories { get; set; }
		[Display(Name = "主要演員")]
		public string? FMovieActors { get; set; }
		[Display(Name = "導演")]
		public string? FMovieDirectors { get; set; }
		public bool FDeleted { get; set; }
	}
	public static class MovieSearchVmExtensions
	{
		public static IEnumerable<MovieSearchVm> ModelsToVms(this IEnumerable<TMovies> movies)
		{
			List<MovieSearchVm> vms = new List<MovieSearchVm>();

			foreach (var movie in movies)
			{
				var vm = ModelToVm(movie);
				vms.Add(vm);
			}
			return vms;
		}

		public static MovieSearchVm ModelToVm(this TMovies movie)
		{
			int len = Math.Min(movie.FMovieIntroduction.Length, 10);

			return new MovieSearchVm()
			{
				FMovieId = movie.FMovieId,
				FMovieIntroduction = movie.FMovieIntroduction,
				PartialIntro = movie.FMovieIntroduction.Trim().Substring(0, len) + "...",
				FMovieName = movie.FMovieName,
				OnDate = movie.FMovieOnDate.ToString("yyyy-MM-dd"),
				OffDate = ((DateTime)movie.FMovieOffDate).ToString("yyyy-MM-dd"),
				FMovieLength = movie.FMovieLength,
				FMovieScore = movie.FMovieScore,
				FMovieImagePath = movie.FMovieImagePath,
				FMovieActors = movie.FMovieActors,
				FMovieDirectors = movie.FMovieDirectors,
				Categories = String.Join(", ", movie.TMovieCategoryDetails
				.Select(t => t.FMoiveCategoryName).ToArray()),
				Level = movie.FMovieLevel.FLevelName,
			};
		}
	}
}
