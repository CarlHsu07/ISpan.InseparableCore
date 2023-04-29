using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.Build.Framework;
using System.ComponentModel;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieSearchVm
	{
		[DisplayName("電影ID")]
		public int FMovieId { get; set; }
		[DisplayName("名稱")]
		public string? FMovieName { get; set; }
		[DisplayName("簡介")]
		public string? PartialIntro { get; set; }
		[DisplayName("簡介")]
		public string? FMovieIntroduction { get; set; }
		[DisplayName("電影分級")]
		public string? Level { get; set; }
		[DisplayName("上映日期")]
		public string? OnDate { get; set; }
		[DisplayName("下映日期")]
		public string? OffDate { get; set; }
		[Required]
		[DisplayName("片長(分鐘)")]
		public int FMovieLength { get; set; }
		[DisplayName("圖片檔路徑")]
		public string? FMovieImagePath { get; set; }
		[DisplayName("會員評分")]
		public decimal? FMovieScore { get; set; } = 0;

		[DisplayName("電影類別")]
		public string? Categories { get; set; }
		[DisplayName("主要演員")]
		public string? FMovieActors { get; set; }
		[DisplayName("導演")]
		public string? FMovieDirectors { get; set; }
	}
	public static class MovieSearchVmExtensions
	{
		public static MovieSearchVm SearchDtoToVm(this MovieSearchDto dto)
		{
			int len = Math.Min(dto.FMovieIntroduction.Length, 10);

			MovieSearchVm vm = new MovieSearchVm()
			{
				FMovieId = dto.FMovieId,
				FMovieIntroduction = dto.FMovieIntroduction,
				PartialIntro = dto.FMovieIntroduction.Trim().Substring(0, len) + "...",
				FMovieName = dto.FMovieName,
				OnDate = dto.FMovieOnDate.ToString("yyyy-MM-dd"),
				OffDate = ((DateTime)dto.FMovieOffDate).ToString("yyyy-MM-dd"),
				FMovieLength = dto.FMovieLength,
				FMovieScore = dto.FMovieScore,
				FMovieImagePath = dto.FMovieImagePath,
				FMovieActors = dto.FMovieActors,
				FMovieDirectors= dto.FMovieDirectors,
			};
			return vm;
		}
	}
}
