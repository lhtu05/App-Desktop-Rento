namespace main.Models
{
    public class Review
    {
        public int RenterID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}
