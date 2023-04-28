using X.PagedList;

namespace ISpan.InseparableCore.ViewModels
{
	public static class GetPage
	{
		//產生頁碼
		public static IPagedList<T> GetPagedProcess<T>(int? page, int pageSize, List<T> movies)
		{
			// 過濾從client傳送過來有問題頁數
			if (page.HasValue && page < 1)
				return null;
			// 從資料庫取得資料
			var listUnpaged = movies;
			IPagedList<T> pagelist = listUnpaged.ToPagedList(page ?? 1, pageSize);
			// 過濾從client傳送過來有問題頁數，包含判斷有問題的頁數邏輯
			if (pagelist.PageNumber != 1 && page.HasValue && page > pagelist.PageCount)
				return null;
			return pagelist;
		}

	}
}
