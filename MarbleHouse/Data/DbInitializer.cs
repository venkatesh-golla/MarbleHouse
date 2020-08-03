using MarbleHouse.Models;
using MarbleHouse.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarbleHouse.Data
{
    public class DbInitializer : IDbInitializer
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async void Initialize()
        {
            _context.Database.Migrate();
            if (_context.Roles.Any(r => r.Name == SD.SuperAdminEndUser))
            {
                return;
            }
            _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                PhoneNumber = "2268996245",
                Name = "Admin_venkatesh",
                EmailConfirmed=true
            },"Admin123!").GetAwaiter().GetResult();

            await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync("admin@gmail.com"), SD.SuperAdminEndUser);
        }
    }
}
