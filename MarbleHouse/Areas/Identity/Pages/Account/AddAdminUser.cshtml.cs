using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarbleHouse.Models;
using MarbleHouse.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarbleHouse.Areas.Identity.Pages.Account
{
    public class AddAdminUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddAdminUserModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> OnGet()
        {
            if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
            }
            if (!await _roleManager.RoleExistsAsync(SD.SuperAdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser));
                var userAdmin = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    PhoneNumber = "2268996245",
                    Name = "Admin_venkatesh"
                };

                var resultUser = await _userManager.CreateAsync(userAdmin, "Admin123!");
                await _userManager.AddToRoleAsync(userAdmin, SD.SuperAdminEndUser);
            }
            if (!await _roleManager.RoleExistsAsync(SD.Customer))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Customer));
            }


            return Page();
        }
    }
}
