using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarbleHouse.Data;
using MarbleHouse.Extensions;
using MarbleHouse.Models;
using MarbleHouse.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MarbleHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };
        }
        public async Task<IActionResult> Index()
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (shoppingCartList!=null && shoppingCartList.Count > 0)
            {
                foreach(int cartItem in shoppingCartList)
                {
                    Products product = _context.Products.Include(p=>p.ProductTypes).Include(s=>s.SpecialTags).Where(p => p.Id == cartItem).FirstOrDefault();
                    ShoppingCartVM.Products.Add(product);
                }
            }
            return View(ShoppingCartVM);  
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexPost()
        {
            List<int> cartItemsList = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate
                .AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour)
                .AddMinutes(ShoppingCartVM.Appointments.AppointmentTime.Minute);

            Appointments appointments = ShoppingCartVM.Appointments;
            _context.Appointments.Add(appointments);
            _context.SaveChanges();

            int appointmentId = appointments.Id;

            foreach (int productId in cartItemsList)
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };
                _context.ProductsSelectedForAppointments.Add(productsSelectedForAppointment);
                
            }
            _context.SaveChanges();

            cartItemsList = new List<int>();
            HttpContext.Session.Set("ssShoppingCart",cartItemsList);

            return RedirectToAction(nameof(AppointmentConfirmation),new { Id=appointmentId});

        }

        public IActionResult Remove(int id)
        {
            List<int> cartItemsList = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(cartItemsList!=null && cartItemsList.Count > 0)
            {
                if (cartItemsList.Contains(id))
                {
                    cartItemsList.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", cartItemsList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AppointmentConfirmation(int id)
        {
            ShoppingCartVM.Appointments = _context.Appointments.Where(a => a.Id == id).FirstOrDefault();
            List<ProductsSelectedForAppointment> productListOfAppointment = _context.ProductsSelectedForAppointments.Where(p => p.AppointmentId == id).ToList();

            foreach(ProductsSelectedForAppointment productAppointment in productListOfAppointment)
            {
                ShoppingCartVM.Products.Add(_context.Products.Include(p => p.ProductTypes).Include(s => s.SpecialTags).Where(p => p.Id == productAppointment.ProductId).FirstOrDefault());
            }

            return View(ShoppingCartVM);
        }

    }
}