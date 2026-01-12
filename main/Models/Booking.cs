namespace main.Models
{
    class Booking
    {
        public int ID { get; set; }

        public int PropertyID { get; set; }

        public int RenterID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
