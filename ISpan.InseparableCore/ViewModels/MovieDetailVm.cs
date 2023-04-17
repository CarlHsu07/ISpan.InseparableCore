using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieDetailVm
	{
		public int FSerialNumber { get; set; }
		public int FMovieId { get; set; }
		public string? MemberName { get; set; }
		public int FMemberId { get; set; }
		public int FScore { get; set; }
		public string? FComment { get; set; }
		public DateTime FPostingDate { get; set; }
		public string? PostDate { get; set; }
		public bool FDeleted { get; set; }

	}
	public static class MovieScoreVmExtensions
	{
		public static MovieDetailVm ModelToVm(this TMovieCommentDetails model)
		{
			return new MovieDetailVm
			{
				FMovieId = model.FMovieId,
				FMemberId = model.FMemberId,
				FSerialNumber = model.FSerialNumber,
				FComment = model.FComment,
				FDeleted = model.FDeleted,
				FPostingDate = model.FPostingDate,
				PostDate = model.FPostingDate.ToString("yy-MM-dd HH:mm:ss"),
			};
		}
		public static TMovieCommentDetails VmToModel(this MovieDetailVm vm)
		{
			return new TMovieCommentDetails
			{
				FMovieId = vm.FMovieId,
				FMemberId = vm.FMemberId,
				FSerialNumber = vm.FSerialNumber,
				FComment = vm.FComment,
				FDeleted = vm.FDeleted,
				FPostingDate = vm.FPostingDate,
			};
		}

		public static List<MovieDetailVm> ModelToVms(this List<TMovieCommentDetails> details)
		{
			List<MovieDetailVm> vms = new List<MovieDetailVm>();
			foreach (var detail in details)
			{
				vms.Add(detail.ModelToVm());
			}
			return vms;
		}
	}
}
