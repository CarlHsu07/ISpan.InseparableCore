namespace ISpan.InseparableCore.ViewModels
{
	public class MovieSearchCondition
	{
		public string? Key { get; set; }
		public int? MovieId { get; set; }
		public int? CategoryId { get; set; }
		public int Page { get; set; } = 1;
		public int? DateCategoryId { get; set; }

	}
}
