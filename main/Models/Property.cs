using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace main.Models
{
    public class Property
    {
        public int ID { get; set; }
        public Host Host { get; set; }
        public Ward Ward { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public ulong Price { get; set; }
        public double Area { get; set; }
        public ulong Deposit { get; set; }
        public string Status { get; set; }
        public string Street { get; set; }
        public string RoomType { get; set; }
        public int Capacity { get; set; }
        public int ViewCount { get; set; }
        public List<string> Amenities { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<string> ImageUrls { get; set; }
        public string StatusColor => Status == "Trống" ? "#4CAF50" :
                                       Status == "Đã thuê" ? "#F44336" : "#FFC107";

    }

}
