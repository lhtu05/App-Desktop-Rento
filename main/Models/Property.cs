using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace main.Models
{
    public class Property
    {
        public int ID { get; set; }

        public Host Host { get; set; }

        public int WardID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; }

        public string Street { get; set; }

        public DateTime CreatedAt { get; set; }
            
    }

}
