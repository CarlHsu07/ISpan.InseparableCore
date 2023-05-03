using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel;

namespace ISpan.InseparableCore.ViewModels
{
	public class CommentVm
	{
		public int FCommentId { get; set; }
		public int FArticleId { get; set; }
		public int FItemNumber { get; set; }
		public int FMemberPk { get; set; }
		[DisplayName("會員Id")]
		public string? FMemberId { get; set; }
		[DisplayName("會員名字")]
		public string? MemberName { get; set; }
		[DisplayName("留言日期")]
		public DateTime? FCommentPostingDate { get; set; }
		[DisplayName("留言日期")]
		public string? PostingDate { get; set; }
		[DisplayName("修改日期")]
		public DateTime? FCommentModifiedDate { get; set; }
		[DisplayName("修改日期")]
		public string? ModifiedDate { get; set; }
		[DisplayName("點讚數")]
		public int FCommentLikes { get; set; }
		[DisplayName("留言內容")]
		public string? FCommentContent { get; set; }
		public bool FDeleted { get; set; }

	}
	public static class CommentVmExtension
	{
		public static CommentVm ModelToVm(this TComments comment)
		{
			return new CommentVm()
			{
				FCommentId = comment.FCommentId,
				FArticleId = comment.FArticleId,
				FMemberPk = comment.FMemberId,
				FCommentPostingDate = comment.FCommentPostingDate,
				PostingDate = comment.FCommentPostingDate.ToString("yyyy-MM-dd HH:mm:ss"),
				FCommentModifiedDate = comment.FCommentModifiedDate,
				ModifiedDate = comment.FCommentModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"),
				FCommentLikes = comment.FCommentLikes,
				FCommentContent = comment.FCommentContent,
				FDeleted = comment.FDeleted,
			};
		}

		public static TComments VmToModel(this CommentVm vm)
		{
			return new TComments()
			{
				FCommentId = vm.FCommentId,
				FArticleId = vm.FArticleId,
				FMemberId = vm.FMemberPk,
				FCommentPostingDate = (DateTime)vm.FCommentPostingDate,
				FCommentModifiedDate = (DateTime)vm.FCommentModifiedDate,
				FCommentLikes = vm.FCommentLikes,
				FCommentContent = vm.FCommentContent,
				FDeleted = vm.FDeleted,
			};
		}

	}

}
