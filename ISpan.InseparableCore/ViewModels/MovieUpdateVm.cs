using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.Build.Framework;
using System.ComponentModel;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieUpdateVm
	{
		[DisplayName("電影ID")]
		public int FMovieId { get; set; }
		[DisplayName("名稱")]
		[Required]
		public string FMovieName { get; set; }
		[Required]
		[DisplayName("簡介")]
		public string FMovieIntroduction { get; set; }
		[Required]
		[DisplayName("電影分級")]
		public int FMovieLevelId { get; set; }
		[DisplayName("上映日期")]
		[Required]
		public DateTime FMovieOnDate { get; set; }
		[DisplayName("下映日期")]
		public DateTime? FMovieOffDate { get; set; }
		[Required]
		[DisplayName("片長(分鐘)")]
		public int FMovieLength { get; set; }
		[DisplayName("圖片檔路徑")]
		public string? FMovieImagePath { get; set; }
		[DisplayName("電影類別")]
		public string? CategoryIds { get; set; }
		public List<int>? CategoryIdsContained { get; set; }
		[DisplayName("主要演員")]
		public string? FMovieActors { get; set; }
		[DisplayName("導演")]
		public string? FMovieDirectors { get; set; }
		[DisplayName("宣傳照")]
		public IFormFile? Image { get; set; }
	}
	public static class MovieUpdateVmExtensions
	{
		public static MovieUpdateVm UpdateDtoToVm(this MovieUpdateDto movie)
		{
			return new MovieUpdateVm()
			{
				FMovieId = movie.FMovieId,
				FMovieIntroduction = movie.FMovieIntroduction,
				FMovieName = movie.FMovieName,
				FMovieLevelId = movie.FMovieLevelId,
				FMovieOnDate = movie.FMovieOnDate,
				FMovieOffDate = movie.FMovieOffDate,
				FMovieLength = movie.FMovieLength,
				FMovieImagePath = movie.FMovieImagePath,
				FMovieActors = movie.FMovieActors,
				FMovieDirectors= movie.FMovieDirectors,
			};
		}
		public static MovieUpdateDto UpdateVmToDto(this MovieUpdateVm vm)
		{
			return new MovieUpdateDto()
			{
				FMovieId = vm.FMovieId,
				FMovieIntroduction = vm.FMovieIntroduction,
				FMovieName = vm.FMovieName,
				FMovieLevelId = vm.FMovieLevelId,
				FMovieOnDate = vm.FMovieOnDate,

				FMovieOffDate = vm.FMovieOffDate,
				FMovieLength = vm.FMovieLength,
				FMovieImagePath = vm.FMovieImagePath,
				FMovieActors = vm.FMovieActors,
				FMovieDirectors = vm.FMovieDirectors,
			};
		}
	}
}
