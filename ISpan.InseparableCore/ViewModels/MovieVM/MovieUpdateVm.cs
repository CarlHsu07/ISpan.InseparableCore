using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieUpdateVm
	{
		[Display(Name = "電影ID")]
		public int FMovieId { get; set; }
		[Display(Name = "名稱")]
		[Required(ErrorMessage = "必填")]
		public string FMovieName { get; set; }
		[Display(Name = "簡介")]
		[Required(ErrorMessage = "必填")]
		public string FMovieIntroduction { get; set; }
		[Display(Name = "電影分級")]
		[Required(ErrorMessage = "必填")]
		public int FMovieLevelId { get; set; }
		[Display(Name = "上映日期")]
		[Required(ErrorMessage = "必填")]
		public DateTime FMovieOnDate { get; set; }
		[Display(Name = "下映日期")]
		public DateTime? FMovieOffDate { get; set; }
		[Display(Name = "片長(分鐘)")]
		[Required(ErrorMessage = "必填")]
		public int FMovieLength { get; set; }
		[Display(Name = "圖片檔路徑")]
		public string? FMovieImagePath { get; set; }
		[Display(Name = "電影類別")]
		public string? CategoryIds { get; set; }
		public List<int>? CategoryIdsContained { get; set; }
		[Display(Name = "主要演員")]
		public string? FMovieActors { get; set; }
		[Display(Name = "導演")]
		public string? FMovieDirectors { get; set; }
		[Display(Name = "宣傳照")]
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
