using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.ViewModels
{
	public class MovieScoreVm
	{
		public int FSerialNumber { get; set; }
		public int FMovieId { get; set; }
		public int FMemberId { get; set; }
		public int FScore { get; set; }

	}
	public static class MovieScoreVmExtensions
	{
		public static MovieScoreVm ModelToVm(this TMovieScoreDetails model)
		{
			return new MovieScoreVm
			{
				FMovieId = model.FMovieId,
				FMemberId = model.FMemberId,
				FSerialNumber = model.FSerialNumber,
				FScore = model.FScore,
			};
		}
		public static TMovieScoreDetails VmToModel(this MovieScoreVm vm)
		{
			return new TMovieScoreDetails
			{
				FMovieId = vm.FMovieId,
				FMemberId = vm.FMemberId,
				FSerialNumber = vm.FSerialNumber,
				FScore = vm.FScore,
			};
		}
	}

}
