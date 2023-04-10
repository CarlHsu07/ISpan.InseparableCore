using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISpan.InseparableCore.Models;
using ISpan.InseparableCoreMVC.ViewModels;

namespace ISpan.InseparableCore.Controllers
{
    public class AdminMember : Controller
    {
        private readonly InseparableContext _context;

        public AdminMember(InseparableContext context)
        {
            _context = context;
        }

        // GET: AdminMember
        public async Task<IActionResult> Index(CQueryKeywordViewModel vm)
        {
            if(string.IsNullOrEmpty(vm.txtKeyword))
            {
                var inseparableContext = _context.TMembers
                .Include(t => t.FAccountStatusNavigation)
                .Include(t => t.FAreaZipCodeNavigation)
                .Include(t => t.FGender);

                return View(await inseparableContext.ToListAsync());
            }
            else
            {
                var inseparableContext = _context.TMembers
                    .Where(m => 
                    m.FFirstName.Contains(vm.txtKeyword) ||
                    m.FLastName.Contains(vm.txtKeyword) ||
                    m.FEmail.Contains(vm.txtKeyword) ||
                    m.FMemberId.Contains(vm.txtKeyword)
                    )
                    .Include(t => t.FAccountStatusNavigation)
                    .Include(t => t.FAreaZipCodeNavigation)
                    .Include(t => t.FGender);

                return View(await inseparableContext.ToListAsync());
            }
        }

        // GET: AdminMember/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var tMembers = await _context.TMembers
                .Include(t => t.FAccountStatusNavigation)
                .Include(t => t.FAreaZipCodeNavigation)
                .Include(t => t.FGender)
                .FirstOrDefaultAsync(m => m.FId == id);
            if (tMembers == null)
            {
                return NotFound();
            }

            return View(tMembers);
        }

        // GET: AdminMember/Create
        public IActionResult Create()
        {
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus");
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName");
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType");
            return View();
        }

        // POST: AdminMember/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FId,FMemberId,FLastName,FFirstName,FEmail,FPasswordHash,FPasswordSalt,FDateOfBirth,FGenderId,FCellphone,FAddress,FAreaZipCode,FPhotoPath,FIntroduction,FAccountStatus,FTotalMemberPoint,FSignUpTime")] TMembers tMembers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tMembers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", tMembers.FAccountStatus);
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", tMembers.FAreaZipCode);
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", tMembers.FGenderId);
            return View(tMembers);
        }

        // GET: AdminMember/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var tMembers = await _context.TMembers.FindAsync(id);
            if (tMembers == null)
            {
                return NotFound();
            }
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", tMembers.FAccountStatus);
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", tMembers.FAreaZipCode);
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", tMembers.FGenderId);
            return View(tMembers);
        }

        // POST: AdminMember/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FId,FMemberId,FLastName,FFirstName,FEmail,FPasswordHash,FPasswordSalt,FDateOfBirth,FGenderId,FCellphone,FAddress,FAreaZipCode,FPhotoPath,FIntroduction,FAccountStatus,FTotalMemberPoint,FSignUpTime")] TMembers tMembers)
        {
            if (id != tMembers.FId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tMembers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TMembersExists(tMembers.FId))
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
            ViewData["FAccountStatus"] = new SelectList(_context.TAccountStatuses, "FStatusId", "FStatus", tMembers.FAccountStatus);
            ViewData["FAreaZipCode"] = new SelectList(_context.TAreas, "FZipCode", "FAreaName", tMembers.FAreaZipCode);
            ViewData["FGenderId"] = new SelectList(_context.TGenders, "FGenderId", "FGenderType", tMembers.FGenderId);
            return View(tMembers);
        }

        // GET: AdminMember/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TMembers == null)
            {
                return NotFound();
            }

            var tMembers = await _context.TMembers
                .Include(t => t.FAccountStatusNavigation)
                .Include(t => t.FAreaZipCodeNavigation)
                .Include(t => t.FGender)
                .FirstOrDefaultAsync(m => m.FId == id);
            if (tMembers == null)
            {
                return NotFound();
            }

            return View(tMembers);
        }

        // POST: AdminMember/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TMembers == null)
            {
                return Problem("Entity set 'InseparableContext.TMembers'  is null.");
            }
            var tMembers = await _context.TMembers.FindAsync(id);
            if (tMembers != null)
            {
                _context.TMembers.Remove(tMembers);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TMembersExists(int id)
        {
          return (_context.TMembers?.Any(e => e.FId == id)).GetValueOrDefault();
        }
    }
}
