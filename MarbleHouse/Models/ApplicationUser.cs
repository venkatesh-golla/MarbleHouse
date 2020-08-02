using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MarbleHouse.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Display(Name="Person Name")]
        public String Name { get; set; }
        [NotMapped]
        public bool IsSuperAdmin { get; set; }
        [NotMapped]
        public bool IsCustomer { get; set; }
    }
}
