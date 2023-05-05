using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieCommemtVm
	{
		public int FSerialNumber { get; set; }
		public int FMovieId { get; set; }
		public string? MemberName { get; set; }
		public string? FMemberId { get; set; }
		public int FMemberPK { get; set; }
		public string? FComment { get; set; }
		public DateTime FPostingDate { get; set; }
		public string? PostDate { get; set; }
		public bool FDeleted { get; set; }

	}
	public static class MovieCommentVmExtensions
	{
		public static MovieCommemtVm ModelToVm(this TMovieCommentDetails model)
		{
			return new MovieCommemtVm
			{
				FMovieId = model.FMovieId,
				FMemberPK = model.FMemberId,
				FSerialNumber = model.FSerialNumber,
				FComment = model.FComment,
				FDeleted = model.FDeleted,
				FPostingDate = model.FPostingDate,
				PostDate = model.FPostingDate.ToString("yyyy-MM-dd HH:mm:ss"),
			};
		}
		public static TMovieCommentDetails VmToModel(this MovieCommemtVm vm)
		{
			return new TMovieCommentDetails
			{
				FMovieId = vm.FMovieId,
				FMemberId = vm.FMemberPK,
				FSerialNumber = vm.FSerialNumber,
				FComment = vm.FComment,
				FDeleted = vm.FDeleted,
				FPostingDate = vm.FPostingDate,
			};
		}
	}
}
