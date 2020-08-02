using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarbleHouse.Data;
using MarbleHouse.Models;
using MarbleHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarbleHouse.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AdminUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }

        public AdminUserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.ApplicationUsers.ToList());
        }

        public async Task<IActionResult> Edit(string id)
        {
            if(id==null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var userFromDb = await _context.ApplicationUsers.FindAsync(id);
            if(userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }

        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(string id)
        {
            if (id != ApplicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = _context.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
                userFromDb.Name = ApplicationUser.Name;
                userFromDb.PhoneNumber = ApplicationUser.PhoneNumber;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ApplicationUser);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var userFromDb = await _context.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string id)
        {

            ApplicationUser userFromDb = _context.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}