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

		public IEnumerable<MovieVm> Search(MovieSearchCondition? condition)
		{
			var movies = context.TMovies.Where(t => t.FDeleted == false).ToList();

			if (condition == null) return ModelToVms(movies);

			//id搜尋
			if (int.TryParse(condition.Key, out int movieId))
			{
				movies = movies.Where(t => t.FMovieId == movieId).ToList();
				return ModelToVms(movies);
			}
			//電影等級
			if (condition.LevelId != 0) movies = movies.Where(t => t.FMovieLevelId == condition.LevelId).ToList();

			//上下映日期
			if (condition.DateCategoryId == 1)//熱映中
			{
				movies = movies.Where(t => t.FMovieOnDate < DateTime.Now
										&& t.FMovieOffDate > DateTime.Now).ToList();
			}
			else if (condition.DateCategoryId == 2)//即將上映
			{
				movies = movies.Where(t => t.FMovieOnDate > DateTime.Now).ToList();
			}
			else if (condition.DateCategoryId == 3)//已下映
			{
				movies = movies.Where(t => t.FMovieOffDate < DateTime.Now).ToList();
			}

			//關鍵字key
			if (!string.IsNullOrEmpty(condition.Key))
			{
				
				movies = movies.Where(t => t.FMovieName.Contains(condition.Key) 
										|| t.FMovieActors.Contains(condition.Key) 
										|| t.FMovieDirectors.Contains(condition.Key)).ToList();
			}
			//電影類別
			if (condition.CategoryId.HasValue && condition.CategoryId != 0)
			{
				var movieCategoryDetails = context.TMovieCategoryDetails
													.Where(t => t.FMovieCategoryId == condition.CategoryId);
				List<int> movieIds = movieCategoryDetails.Select(t => t.FMovieId).ToList();

				movies = movies.Where(t => movieIds.Contains(t.FMovieId)).ToList();
			}

			return ModelToVms(movies);
		}
		public IEnumerable<MovieVm> ModelToVms(IEnumerable<TMovies> movies)
		{
			List<MovieVm> vms = new List<MovieVm>();
			foreach (var movie in movies)
			{
				MovieVm vm = movie.ModelToVm();
				vm.Level = context.TMovies.Include(t => t.FMovieLevelId)
					.FirstOrDefault(t => t.FMovieId == vm.FMovieId).FMovieLevel.FLevelName;

				//獲得電影類別
				IEnumerable<TMovieCategoryDetails> categorydetails = context.TMovieCategoryDetails
																.Where(t => t.FMovieId == vm.FMovieId);
				//context.Database.CloseConnection();
				if (categorydetails != null)
				{
					List<int> categoryIds = categorydetails.Select(t => t.FMovieCategoryId).ToList();

					List<string> categories = context.TMovieCategories
						.Where(t => categoryIds.Contains(t.FMovieCategoryId))
						.Select(t => t.FMovieCategoryName).ToList();
					vm.Categories = String.Join(", ", categories.ToArray());
				}
				vms.Add(vm);
			}
			return vms;
		}

		public MovieVm GetVmById(int id)
		{
			TMovies movie = context.TMovies.FirstOrDefault(t => t.FMovieId == id);
			MovieVm vm = movie.ModelToVm();
			vm.Level = context.TMovies.Include(t => t.FMovieLevel)
				.FirstOrDefault(t => t.FMovieId == vm.FMovieId).FMovieLevel.FLevelName;

			//獲得電影類別
			IEnumerable<TMovieCategoryDetails> categorydetails = context.TMovieCategoryDetails
															.Where(t => t.FMovieId == vm.FMovieId);
			//context.Database.CloseConnection();
			if (categorydetails != null)
			{
				List<int> categoryIds = categorydetails.Select(t => t.FMovieCategoryId).ToList();

				List<string> categories = context.TMovieCategories
					.Where(t => categoryIds.Contains(t.FMovieCategoryId))
					.Select(t => t.FMovieCategoryName).ToList();
				vm.Categories = String.Join(", ", categories.ToArray());
			}
			return vm;
		}
		public async Task CreateAsync(MovieVm vm)
		{
			//新增Movie
			TMovies movie = vm.VmToModel();
			if (vm.Image != null)
			{
				string imageName = Guid.NewGuid().ToString() + ".jpg";
				string path = enviro.WebRootPath + "/images/" + imageName;
				vm.Image.CopyTo(new FileStream(path, FileMode.Create));
				movie.FMovieImagePath = imageName;
			}
			context.Add(movie);
			//要先作已取得FMoviedId，以新增MovieCategories
			context.SaveChanges();

			//新增MovieCategories
			int movieId = context.TMovies.First(t => t.FMovieName == vm.FMovieName).FMovieId;
			if (!string.IsNullOrEmpty(vm.CategoryIds))
			{
				List<int> categoryIds = vm.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
														.Select(i => int.Parse(i)).ToList();
				foreach (var id in categoryIds)
				{
					TMovieCategoryDetails detail = new TMovieCategoryDetails()
					{
						FMovieId = movieId,
						FMovieCategoryId = id
					};
					context.Add(detail);
				}
			}
			await context.SaveChangesAsync();
		}
		public async Task UpdateAsync(MovieVm vm)
		{

			TMovies movie = context.TMovies.Find(vm.FMovieId);
			if (movie == null) return;
			movie.FMovieName = vm.FMovieName;
			movie.FMovieIntroduction = vm.FMovieIntroduction;
			movie.FMovieLevelId = vm.FMovieLevelId;
			movie.FMovieOnDate = vm.FMovieOnDate;
			movie.FMovieOffDate = vm.FMovieOffDate;
			movie.FMovieLength = vm.FMovieLength;
			movie.FMovieActors = vm.FMovieActors;
			movie.FMovieDirectors = vm.FMovieDirectors;

			if (vm.Image != null)
			{
				string imageName = Guid.NewGuid().ToString() + ".jpg";
				string path = enviro.WebRootPath + "/images/" + imageName;
				vm.Image.CopyTo(new FileStream(path, FileMode.Create));
				movie.FMovieImagePath = imageName;
			}

			context.Update(movie);
			context.SaveChanges();

			if (!string.IsNullOrEmpty(vm.CategoryIds))
			{
				IEnumerable<TMovieCategoryDetails> categoryDetails = context.TMovieCategoryDetails.Where(t => t.FMovieId == vm.FMovieId);
				foreach (TMovieCategoryDetails detail in categoryDetails)
				{
					context.Remove(detail);
				}

				List<int> categoryIds = vm.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
														.Select(i => int.Parse(i)).ToList();
				foreach (var categoryId in categoryIds)
				{
					TMovieCategoryDetails detail = new TMovieCategoryDetails()
					{
						FMovieId = vm.FMovieId,
						FMovieCategoryId = categoryId,
					};
					context.Add(detail);
				}
			}
			await context.SaveChangesAsync();

		}
		public async Task DeleteAsync(int movieId)
		{
			var movie = await context.TMovies.FindAsync(movieId);
			if (movie != null)
			{
				movie.FDeleted = true;
				context.Update(movie);
				await context.SaveChangesAsync();
			}
		}
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
            var data = context.TMovies.Where(t => t.FMovieOffDate > today && t.FMovieOnDate < today && t.FDeleted == false).OrderByDescending(t => t.FMovieOffDate).Take(6);

			return data;
        }
        public IEnumerable<TMovies> Soon()
        {
            DateTime today = DateTime.Now.Date;
            var data = context.TMovies.Where(t => t.FMovieOnDate > today && t.FDeleted == false).OrderBy(t => t.FMovieOffDate).Take(6); 

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
