﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarbleHouse.Models.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public Appointments Appointments { get; set; }
        public List<ApplicationUser> SalesPerson { get; set; }
        public List<Products> Products { get; set; }
    }
}
