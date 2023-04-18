namespace ISpan.InseparableCore.ViewModels
{
	public class ArticleSearchCondition
	{
		public int? ArticleId { get; set; }
		public string? Key { get; set; }
		public int? CategoryId { get; set; }
		public int Page { get; set; } = 1;

	}
}
