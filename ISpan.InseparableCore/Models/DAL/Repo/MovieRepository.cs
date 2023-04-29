using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.ViewModels;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace ISpan.InseparableCore.Models.DAL
{
	public class MovieRepository
	{
		private readonly InseparableContext context;
		private readonly IWebHostEnvironment enviro;

		public MovieRepository(InseparableContext context, IWebHostEnvironment enviro)
		{
			this.context = context;
			this.enviro = enviro;
		}

		public IEnumerable<MovieEntity> Search(MovieSearchCondition? condition)
		{
			var movies = context.TMovies.Where(t => t.FDeleted == false);

			if (condition == null) return ModelsToEntities(movies);

			//id搜尋
			if (int.TryParse(condition.Key, out int movieId))
			{
				movies = movies.Where(t => t.FMovieId == movieId);
				return ModelsToEntities(movies);
			}
			//電影等級
			if (condition.LevelId != 0) movies = movies.Where(t => t.FMovieLevelId == condition.LevelId);

			//上下映日期
			if (condition.DateCategoryId == 1)//熱映中
			{
				movies = movies.Where(t => t.FMovieOnDate < DateTime.Now
										&& t.FMovieOffDate > DateTime.Now);
			}
			else if (condition.DateCategoryId == 2)//即將上映
			{
				movies = movies.Where(t => t.FMovieOnDate > DateTime.Now);
			}
			else if (condition.DateCategoryId == 3)//已下映
			{
				movies = movies.Where(t => t.FMovieOffDate < DateTime.Now);
			}

			//關鍵字key
			if (!string.IsNullOrEmpty(condition.Key))
			{
				
				movies = movies.Where(t => t.FMovieName.Contains(condition.Key) 
										|| t.FMovieActors.Contains(condition.Key) 
										|| t.FMovieDirectors.Contains(condition.Key));
			}
			//電影類別
			if (condition.CategoryId.HasValue && condition.CategoryId != 0)
			{
				var movieCategoryDetails = context.TMovieCategoryDetails
													.Where(t => t.FMovieCategoryId == condition.CategoryId);
				List<int> movieIds = movieCategoryDetails.Select(t => t.FMovieId).ToList();

				movies = movies.Where(t => movieIds.Contains(t.FMovieId));
			}

			return ModelsToEntities(movies);
		}
		public MovieEntity ModelToEntity(TMovies movie)
		{
			return new MovieEntity()
			{
				FMovieId = movie.FMovieId,
				FMovieIntroduction = movie.FMovieIntroduction,
				FMovieName = movie.FMovieName,
				FMovieLevelId = movie.FMovieLevelId,
				FMovieOnDate = movie.FMovieOnDate,
				FMovieOffDate = movie.FMovieOffDate,
				FMovieLength = movie.FMovieLength,
				FMovieScore = movie.FMovieScore,
				FMovieImagePath = movie.FMovieImagePath,
				FMovieActors = movie.FMovieActors,
				FMovieDirectors = movie.FMovieDirectors,
				FDeleted = movie.FDeleted,
			};
		}

		public IEnumerable<MovieEntity> ModelsToEntities(IEnumerable<TMovies> movies)
		{
			List<MovieEntity> entities = new List<MovieEntity>();
			foreach (var movie in movies)
			{
				MovieEntity vm = movie.ModelToEntity();
				entities.Add(vm);
			}
			return entities;
		}
		public string GetMovieLevel(int levelId)
		{
			return context.TMovieLevels.Find(levelId).FLevelName;
		}
		public string GetCategories(int movieId)
		{
			//if (categorydetails == null) return String.Empty;

			List<string> categories = context.TMovieCategoryDetails.Where(t => t.FMovieId == movieId)
				.Select(t => t.FMoiveCategoryName).ToList();
			return String.Join(", ", categories.ToArray());
		}
		public MovieEntity GetByMovieId(int movieId)
		{
			TMovies movie = context.TMovies.Find(movieId);
			if(movie == null) return null;

			return movie.ModelToEntity();
		}
		public int GetMovieId(string movieName)
		{
			TMovies movie = context.TMovies.FirstOrDefault(t => t.FMovieName.Equals(movieName));

			return movie.FMovieId;
		}
		public async Task Create(MovieEntity entity)
		{
			//新增Movie
			TMovies movie = new TMovies();
			movie.FMovieName = entity.FMovieName;
			movie.FMovieIntroduction = entity.FMovieIntroduction;
			movie.FMovieLevelId = entity.FMovieLevelId;
			movie.FMovieOnDate = entity.FMovieOnDate;
			movie.FMovieOffDate = entity.FMovieOffDate;

			movie.FMovieLength = entity.FMovieLength;
			movie.FMovieImagePath = entity.FMovieImagePath;
			movie.FMovieActors = entity.FMovieActors;
			movie.FMovieDirectors = entity.FMovieDirectors;

			context.Add(movie);
			context.SaveChanges();
		}
		public async Task Update(MovieEntity entity)
		{

			TMovies movie = context.TMovies.Find(entity.FMovieId);
			if (movie == null) return;

			movie.FMovieName = entity.FMovieName;
			movie.FMovieIntroduction = entity.FMovieIntroduction;
			movie.FMovieLevelId = entity.FMovieLevelId;
			movie.FMovieOnDate = entity.FMovieOnDate;
			movie.FMovieOffDate = entity.FMovieOffDate;

			movie.FMovieLength = entity.FMovieLength;
			movie.FMovieImagePath = entity.FMovieImagePath;
			movie.FMovieActors = entity.FMovieActors;
			movie.FMovieDirectors = entity.FMovieDirectors;

			context.Update(movie);
			context.SaveChanges();
		}
		public void UpdateCategoryDetail(int movieId, string? categoryIds)
		{
			if (string.IsNullOrEmpty(categoryIds)) return;
			IEnumerable<TMovieCategoryDetails> categoryDetails = context.TMovieCategoryDetails.Where(t => t.FMovieId == movieId);
			foreach (TMovieCategoryDetails detail in categoryDetails)
			{
				context.Remove(detail);
			}

			CreateCategoryDetail(movieId, categoryIds);
			context.SaveChanges();
		}
		public void CreateCategoryDetail(int movieId, string? categoryIds)
		{
			if (string.IsNullOrEmpty(categoryIds)) return;

			List<int> ListCategoryId = categoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
													.Select(i => int.Parse(i)).ToList();
			foreach (var categoryId in ListCategoryId)
			{
				TMovieCategoryDetails detail = new TMovieCategoryDetails()
				{
					FMovieId = movieId,
					FMovieCategoryId = categoryId,
					FMoiveCategoryName = context.TMovieCategories
					.SingleOrDefault(t => t.FMovieCategoryId == categoryId).FMovieCategoryName
				};
				context.Add(detail);
			}
			context.SaveChanges();
		}

		public async Task Delete(int movieId)
		{
			var movie = await context.TMovies.FindAsync(movieId);
			if (movie != null)
			{
				movie.FDeleted = true;
				context.Update(movie);
				await context.SaveChangesAsync();
			}
		}
		/// <summary>
		/// /////////
		/// </summary>
		/// <param name="movie"></param>
		/// <returns></returns>
        public TMovies GetOneMovie(int? movie)
        {
            var data = context.TMovies.FirstOrDefault(t => t.FMovieId == movie);
            return data;
        }
		public IEnumerable<TMovies> GetByOffDay()
		{
			var today = DateTime.Now.Date;
			var data = context.TMovies.Where(t => t.FMovieOffDate >= today && t.FDeleted==false);

			return data;
		}
		public IEnumerable<TMovies> Showing()
		{
            DateTime today = DateTime.Now.Date;
            var data = context.TMovies.Where(t => t.FMovieOffDate > today && t.FMovieOnDate < today && t.FDeleted == false)
				.OrderByDescending(t => t.FMovieOffDate).Take(6);

			return data;
        }
        public IEnumerable<TMovies> Soon()
        {
            DateTime today = DateTime.Now.Date;
            var data = context.TMovies.Where(t => t.FMovieOnDate > today && t.FDeleted == false)
				.OrderBy(t => t.FMovieOffDate).Take(6); 

            return data;
        }
		public IEnumerable<TMovies> Movie(string keyword)
		{
			if (string.IsNullOrEmpty(keyword))
				return null;

			var movies = context.TMovies.Where(t => t.FMovieName.Contains(keyword)
                                            || t.FMovieActors.Contains(keyword)
                                            || t.FMovieDirectors.Contains(keyword)).ToList();

			return movies;
        }
    }
}
