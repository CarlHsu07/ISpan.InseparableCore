namespace ISpan.InseparableCore.ViewModels
{
	public class MovieCreateVm
	{
		public int FMovieId { get; set; }
		public string FMovieName { get; set; }
		public string FMovieIntroduction { get; set; }
		public int FMovieLevelId { get; set; }
		public DateTime FMovieOnDate { get; set; }
		public DateTime? FMovieOffDate { get; set; }
		public int FMovieLength { get; set; }
		public string FMovieImagePath { get; set; }
		public int FMovieScore { get; set; }
		public string CategoryIds { get; set; }
		public IFormFile Image { get; set; }
	}
}
