using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarbleHouse.Data;
using MarbleHouse.Models;
using MarbleHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MarbleHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpecialTagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var specialTags= _context.SpecialTags.ToList();
            return View(specialTags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTags specialTags)
        {
            if (ModelState.IsValid)
            {
                _context.SpecialTags.Add(specialTags);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialTags);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var specialTag = await _context.SpecialTags.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }
            return View(specialTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SpecialTags specialTags)
        {
            if (id != specialTags.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                 _context.SpecialTags.Update(specialTags);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialTags);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var specialTags = await _context.SpecialTags.FindAsync(id);
            if (specialTags == null)
            {
                return NotFound();
            }
            return View(specialTags);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var specialTags = await _context.SpecialTags.FindAsync(id);
            if (specialTags == null)
            {
                return NotFound();
            }
            return View(specialTags);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialTag = await _context.SpecialTags.FindAsync(id);
            _context.SpecialTags.Remove(specialTag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}