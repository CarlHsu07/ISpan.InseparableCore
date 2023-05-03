using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System.ComponentModel.DataAnnotations;


namespace ISpan.InseparableCore.ViewModels
{
	public class MovieCreateVm
	{
		[Display(Name = "名稱")]
		[Required(ErrorMessage = "必填")]
		public string? FMovieName { get; set; }
		[Required(ErrorMessage = "必填")]
		[Display(Name = "簡介")]
		public string? FMovieIntroduction { get; set; }
		[Required(ErrorMessage = "必填")]
		[Display(Name = "電影分級")]
		public int FMovieLevelId { get; set; }
		[Required(ErrorMessage = "必填")]
		[Display(Name = "上映日期")]
		public DateTime FMovieOnDate { get; set; }
		[Display(Name = "下映日期")]
		public DateTime? FMovieOffDate { get; set; }
		[Required(ErrorMessage = "必填")]
		[Display(Name = "片長(分鐘)")]
		public int FMovieLength { get; set; }
		[Display(Name = "電影類別")]
		public string? CategoryIds { get; set; }

		[Display(Name = "主要演員")]
		public string? FMovieActors { get; set; }
		[Display(Name = "導演")]
		public string? FMovieDirectors { get; set; }
		[Display(Name = "宣傳照")]
		public IFormFile? Image { get; set; }
	}
	public static class MovieCreateVmExtensions
	{
		public static MovieCreateDto CreateVmToDto(this MovieCreateVm vm)
		{
			return new MovieCreateDto
			{
				FMovieIntroduction = vm.FMovieIntroduction,
				FMovieName = vm.FMovieName,
				FMovieLevelId = vm.FMovieLevelId,
				FMovieOnDate = vm.FMovieOnDate,
				FMovieOffDate = vm.FMovieOnDate.AddMonths(1),
				FMovieLength = vm.FMovieLength,
				FMovieActors = vm.FMovieActors,
				FMovieDirectors = vm.FMovieDirectors,
			};
		}
	}
}
