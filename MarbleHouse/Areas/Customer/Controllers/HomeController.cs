using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MarbleHouse.Models;
using MarbleHouse.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using MarbleHouse.Extensions;

namespace MarbleHouse.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productList =await _context.Products.Include(p => p.ProductTypes).Include(s => s.SpecialTags).ToListAsync();
            if (productList == null)
            {
                return NotFound();
            }
            return View(productList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var product =await _context.Products.Include(p => p.ProductTypes).Include(s => s.SpecialTags).Where(p=>p.Id==id).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost,ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (shoppingCartList == null)
            {
                shoppingCartList = new List<int>();
            }
            shoppingCartList.Add(id);
            HttpContext.Session.Set("ssShoppingCart", shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Remove(int id)
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (shoppingCartList.Count > 0)
            {
                if (shoppingCartList.Contains(id))
                {
                    shoppingCartList.Remove(id);
                }
            }
            HttpContext.Session.Set("ssShoppingCart", shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
