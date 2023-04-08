using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models;
using ISpan.InseparableCore.ViewModels;

namespace ISpan.InseparableCore.Controllers
{
	public class TMovieCategoryDetailsController : Controller
	{
		private readonly InseparableContext _context;

		public TMovieCategoryDetailsController(InseparableContext context)
		{
			_context = context;
		}


		// GET: TMovieCategoryDetails
		public async Task<IActionResult> Index()
		{
			var inseparableContext = _context.TMovieCategoryDetails.Include(t => t.FMovie).Include(t => t.FMovieCategory);
			return View(await inseparableContext.ToListAsync());
		}

		// GET: TMovieCategoryDetails/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.TMovieCategoryDetails == null)
			{
				return NotFound();
			}

			var tMovieCategoryDetails = await _context.TMovieCategoryDetails
				.Include(t => t.FMovie)
				.Include(t => t.FMovieCategory)
				.FirstOrDefaultAsync(m => m.FSerialNumber == id);
			if (tMovieCategoryDetails == null)
			{
				return NotFound();
			}

			return View(tMovieCategoryDetails);
		}

		// GET: TMovieCategoryDetails/Create
		public IActionResult Create()
		{
			ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieName");
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");
			return View();
		}

		// POST: TMovieCategoryDetails/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(MovieCategoryDetailCreateVm detailVm)
		{
			if (ModelState.IsValid)
			{
				List<int> categoryIds = detailVm.CategoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i)).ToList();
				foreach (var id in categoryIds)
				{
					TMovieCategoryDetails detail = new TMovieCategoryDetails()
					{
						FMovieId = detailVm.FMovieId,
						FMovieCategoryId = id
					};
					_context.Add(detail);
				}
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieName", detailVm.FMovieId);
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName");

			return View(detailVm);
		}

		// GET: TMovieCategoryDetails/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.TMovieCategoryDetails == null)
			{
				return NotFound();
			}

			var tMovieCategoryDetails = await _context.TMovieCategoryDetails.FindAsync(id);
			if (tMovieCategoryDetails == null)
			{
				return NotFound();
			}
			ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieName", tMovieCategoryDetails.FMovieId);
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", tMovieCategoryDetails.FMovieCategoryId);
			return View(tMovieCategoryDetails);
		}

		// POST: TMovieCategoryDetails/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("FSerialNumber,FMovieId,FMovieCategoryId")] TMovieCategoryDetails tMovieCategoryDetails)
		{
			if (id != tMovieCategoryDetails.FSerialNumber)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(tMovieCategoryDetails);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TMovieCategoryDetailsExists(tMovieCategoryDetails.FSerialNumber))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["FMovieId"] = new SelectList(_context.TMovies, "FMovieId", "FMovieName", tMovieCategoryDetails.FMovieId);
			ViewData["FMovieCategoryId"] = new SelectList(_context.TMovieCategories, "FMovieCategoryId", "FMovieCategoryName", tMovieCategoryDetails.FMovieCategoryId);
			return View(tMovieCategoryDetails);
		}

		// GET: TMovieCategoryDetails/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.TMovieCategoryDetails == null)
			{
				return NotFound();
			}

			var tMovieCategoryDetails = await _context.TMovieCategoryDetails
				.Include(t => t.FMovie)
				.Include(t => t.FMovieCategory)
				.FirstOrDefaultAsync(m => m.FSerialNumber == id);
			if (tMovieCategoryDetails == null)
			{
				return NotFound();
			}

			return View(tMovieCategoryDetails);
		}

		// POST: TMovieCategoryDetails/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.TMovieCategoryDetails == null)
			{
				return Problem("Entity set 'InseparableContext.TMovieCategoryDetails'  is null.");
			}
			var tMovieCategoryDetails = await _context.TMovieCategoryDetails.FindAsync(id);
			if (tMovieCategoryDetails != null)
			{
				_context.TMovieCategoryDetails.Remove(tMovieCategoryDetails);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool TMovieCategoryDetailsExists(int id)
		{
			return (_context.TMovieCategoryDetails?.Any(e => e.FSerialNumber == id)).GetValueOrDefault();
		}
	}
}
