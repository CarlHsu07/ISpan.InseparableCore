using ISpan.InseparableCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
	public class MovieScoreRepository
	{
		private readonly InseparableContext context;

		public MovieScoreRepository(InseparableContext context)
		{
			this.context = context;
		}
		public TMovieScoreDetails? GetDetail(int movieId, int userId)
		{
			var detail = context.TMovieScoreDetails.
				FirstOrDefault(t => t.FMemberId == userId && t.FMovieId == movieId);
			return detail;
		}
		public int GetScore(int movieId, int userId)
		{
			var detail = GetDetail(movieId, userId);
			if (detail == null) return 0;

			return detail.FScore;
		}

		public void Create(MovieScoreVm scoreVm)
		{
			TMovieScoreDetails score = scoreVm.VmToModel();
			context.Add(score);
			context.SaveChanges();
			UpdateMovieScore(score.FMovieId);
		}
		public void Update(MovieScoreVm scoreVm)
		{
			var detailInDb = GetDetail(scoreVm.FMovieId, scoreVm.FMemberId);
			if (detailInDb == null) return;
			detailInDb.FScore = scoreVm.FScore;
			context.Update(detailInDb);
			context.SaveChanges();
			UpdateMovieScore(scoreVm.FMovieId);
		}
		public void UpdateMovieScore(int movieId)
		{
			var movie = context.TMovies.Find(movieId);
			movie.FMovieScore = (decimal)context.TMovieScoreDetails
				.Where(t => t.FMovieId == movieId).Average(t => t.FScore);
			context.Update(movie);
			context.SaveChanges();
		}
		public decimal GetMovieScore(int movieId)
		{
			var movie = context.TMovies.Find(movieId);
			return movie.FMovieScore;
		}
		
	}
}
