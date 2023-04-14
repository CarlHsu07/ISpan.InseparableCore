namespace ISpan.InseparableCore.ViewModels
{
	public class MovieDateCategory
	{
		public int Id { get; set; }
		public string? DateCategory { get; set; }
	}
	public static class MovieDateCategoryExtension
	{
		public static List<MovieDateCategory> ToSelectList(this List<string> DateCategories)
		{
			List<MovieDateCategory> dateCategorySelectList = new List<MovieDateCategory>();
			for (int i = 0; i < DateCategories.Count; i++)
			{
				dateCategorySelectList.Add(new MovieDateCategory { Id = i, DateCategory = DateCategories[i] });
			}

			return dateCategorySelectList;
		}
	}
}
