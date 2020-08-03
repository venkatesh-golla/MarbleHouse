using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarbleHouse.Models
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        public string urlParameter { get; set; }


    }
}
