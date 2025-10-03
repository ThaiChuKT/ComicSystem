namespace ComicSystem.Models
{
    public class Rental
    {
        public int RentalId { get; set; }
        public int CustomerId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public Customer Customer { get; set; }
        public ICollection<RentalDetail> RentalDetails { get; set; }
    }
}