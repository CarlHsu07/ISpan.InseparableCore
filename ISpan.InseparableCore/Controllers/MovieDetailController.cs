using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.Models.DAL.Repo;
using ISpan.InseparableCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Text.Json;


namespace ISpan.InseparableCore.Controllers
{
	public class MovieDetailController : SuperController
	{
		private readonly InseparableContext _context;
		private readonly MovieCommentRepository commentRepo;
		private readonly MovieScoreRepository scoreRepo;
		public MovieDetailController(InseparableContext context)
		{
			_context = context;
			commentRepo = new MovieCommentRepository(context);
			scoreRepo = new MovieScoreRepository(context);
		}

		[HttpPost]
		public async Task<IActionResult> MovieComment(MovieCommemtVm comment)
		{
			comment.FMemberPk = _user.FId;
			List<MovieCommemtVm> vms = new List<MovieCommemtVm>();
			//無參數=>預設顯示
			if (string.IsNullOrEmpty(comment.FComment))
			{
			}
			else if (comment.FSerialNumber != 0 || comment.FDeleted)//comment已存在=>跟新
			{
				await commentRepo.UpdateAsync(comment);
			}
			else // 新comment=>新增
			{
				await commentRepo.CreateAsync(comment);
			}

			vms = commentRepo.Search(comment.FMovieId).ToList();
			return Ok(new
			{
				Vm = vms,
				UserId = _user.FId,
			}.
			ToJson());
		}
		[HttpPost]
		public IActionResult ShowOwnScore(int movieId)
		{
			int score = scoreRepo.GetScore(movieId, _user.FId);

			return Ok(score);
		}
		[HttpPost]
		public IActionResult MovieScore(MovieScoreVm score)
		{
			score.FMemberId = _user.FId;

			TMovieScoreDetails? scoreInDb = scoreRepo.GetDetail(score.FMovieId, score.FMemberId);
			if (scoreInDb == null)
			{
				scoreRepo.Create(score);
			}
			else
			{
				scoreRepo.Update(score);
			}

			return Ok(scoreRepo.GetMovieScore(score.FMovieId).ToString("f1"));
		}

	}
}
