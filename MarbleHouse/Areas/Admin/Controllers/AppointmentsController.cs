using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MarbleHouse.Data;
using MarbleHouse.Models;
using MarbleHouse.Models.ViewModels;
using MarbleHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarbleHouse.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.AdminEndUser +","+SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private int PageSize = 2;
        public AppointmentViewModel AppointmentViewModel;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
            AppointmentViewModel = new AppointmentViewModel()
            {
                Appointments = new List<Appointments>()
            };
        }
        public async Task<IActionResult> Index(string searchName=null, string searchEmail=null,string searchPhone=null, string searchDate=null,int productPage=1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            StringBuilder param =new StringBuilder();
            param.Append("/Admin/Appointments?productPage=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchDate=");
            if (searchDate != null)
            {
                param.Append(searchDate);
            }
            AppointmentViewModel.Appointments = _context.Appointments.Include(s => s.SalesPerson).ToList();
            if (User.IsInRole(SD.AdminEndUser))
            {
                AppointmentViewModel.Appointments = AppointmentViewModel.Appointments.Where(s => s.SalesPersonId == claim.Value).ToList();
            }

            if (searchName != null)
            {
                AppointmentViewModel.Appointments = AppointmentViewModel.Appointments.Where(c => c.CustomerName.ToLower().Contains(searchName)).ToList();
            }

            if (searchEmail != null)
            {
                AppointmentViewModel.Appointments = AppointmentViewModel.Appointments.Where(c => c.CustomerEMail.ToLower().Contains(searchEmail)).ToList();
            }
            if (searchPhone != null)
            {
                AppointmentViewModel.Appointments = AppointmentViewModel.Appointments.Where(c => c.CustomerPhoneNumber.ToLower().Contains(searchPhone)).ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    AppointmentViewModel.Appointments = AppointmentViewModel.Appointments.Where(c => c.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {

                }
            }

            var count = AppointmentViewModel.Appointments.Count;
            AppointmentViewModel.Appointments = AppointmentViewModel.Appointments.OrderBy(p => p.AppointmentDate)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();
            AppointmentViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParameter = param.ToString()
            };

            return View(AppointmentViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            { 
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _context.Products
                                                   join a in _context.ProductsSelectedForAppointments
                                                   on p.Id equals a.ProductId
                                                   where a.AppointmentId == id
                                                   select p).Include("ProductTypes");
            AppointmentDetailsViewModel objAppointmentVm = new AppointmentDetailsViewModel()
            {
                Appointments = _context.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _context.ApplicationUsers.ToList(),
                Products=productList.ToList()
            };

            return View(objAppointmentVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel objAppointmentVM)
        {
            if (ModelState.IsValid)
            {
                objAppointmentVM.Appointments.AppointmentDate = objAppointmentVM.Appointments.AppointmentDate
                    .AddHours(objAppointmentVM.Appointments.AppointmentTime.Hour)
                    .AddMinutes(objAppointmentVM.Appointments.AppointmentTime.Minute);

                var appointmentFromDb = _context.Appointments.Where(a => a.Id == objAppointmentVM.Appointments.Id).FirstOrDefault();

                appointmentFromDb.CustomerName = objAppointmentVM.Appointments.CustomerName;
                appointmentFromDb.CustomerEMail = objAppointmentVM.Appointments.CustomerEMail;
                appointmentFromDb.CustomerPhoneNumber = objAppointmentVM.Appointments.CustomerPhoneNumber;
                appointmentFromDb.AppointmentDate = objAppointmentVM.Appointments.AppointmentDate;
                appointmentFromDb.isConfirmed = objAppointmentVM.Appointments.isConfirmed;

                if (User.IsInRole(SD.SuperAdminEndUser))
                 {
                    appointmentFromDb.SalesPersonId = objAppointmentVM.Appointments.SalesPersonId;
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(objAppointmentVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _context.Products
                                                      join a in _context.ProductsSelectedForAppointments
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");
            AppointmentDetailsViewModel objAppointmentVm = new AppointmentDetailsViewModel()
            {
                Appointments = _context.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _context.ApplicationUsers.ToList(),
                Products = productList.ToList()
            };

            return View(objAppointmentVm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in _context.Products
                                                      join a in _context.ProductsSelectedForAppointments
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");
            AppointmentDetailsViewModel objAppointmentVm = new AppointmentDetailsViewModel()
            {
                Appointments = _context.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _context.ApplicationUsers.ToList(),
                Products = productList.ToList()
            };

            return View(objAppointmentVm);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var appointment =await _context.Appointments.FindAsync(id);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

