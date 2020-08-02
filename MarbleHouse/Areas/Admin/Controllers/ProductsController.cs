using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarbleHouse.Data;
using MarbleHouse.Models;
using MarbleHouse.Models.ViewModels;
using MarbleHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarbleHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            ProductsVM = new ProductsViewModel()
            {
                ProductTypes = _context.ProductTypes.ToList(),
                SpecialTags = _context.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            var products =await _context.Products.Include(p => p.ProductTypes).Include(s => s.SpecialTags).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            return View(ProductsVM);
        }

       [HttpPost,ActionName("Create")]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }
            _context.Products.Add(ProductsVM.Products);
            await _context.SaveChangesAsync();

            //Image
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productFromDb = _context.Products.Find(ProductsVM.Products.Id);

            if (files.Count != 0)
            {
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);
                using (var filestream = new FileStream(Path.Combine(uploadPath, ProductsVM.Products.Id + extension), FileMode.Create)) 
                {
                    files[0].CopyTo(filestream);
                } ;
                productFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extension;
            }
            else
            {
                var uploadsPath = Path.Combine(webRootPath, SD.ImageFolder+@"\"+SD.DefaultProductImage);
                System.IO.File.Copy(uploadsPath, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png");
                productFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductsVM.Products = await _context.Products.Include(p => p.ProductTypes).Include(s => s.SpecialTags).SingleOrDefaultAsync(m => m.Id == id);
            if (ProductsVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = _context.Products.Where(p => p.Id == ProductsVM.Products.Id).FirstOrDefault();

                if(files.Count>0 && files[0] != null)
                {
                    var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploadPath, ProductsVM.Products.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploadPath, ProductsVM.Products.Id + extension_old));
                    }

                    using (var filestream = new FileStream(Path.Combine(uploadPath, ProductsVM.Products.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    };
                    ProductsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extension_new;
                }

                if (ProductsVM.Products.Image != null)
                {
                    productFromDb.Image = ProductsVM.Products.Image;
                }

                productFromDb.Name = ProductsVM.Products.Name;
                productFromDb.Price = ProductsVM.Products.Price;
                productFromDb.Available = ProductsVM.Products.Available;
                productFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
                productFromDb.SpecialTagId = ProductsVM.Products.SpecialTagId;
                productFromDb.ShadeColor = ProductsVM.Products.ShadeColor;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ProductsVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductsVM.Products = await _context.Products.Include(p => p.ProductTypes).Include(s => s.SpecialTags).SingleOrDefaultAsync(m => m.Id == id);
            if (ProductsVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductsVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductsVM.Products = await _context.Products.Include(p => p.ProductTypes).Include(s => s.SpecialTags).SingleOrDefaultAsync(m => m.Id == id);
            if (ProductsVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductsVM);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Products products = await _context.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }
            else
            {
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(products.Image);
                if (System.IO.File.Exists(Path.Combine(uploadPath, products.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploadPath, products.Id + extension));
                }
                _context.Products.Remove(products);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }
    }
}