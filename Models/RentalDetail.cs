namespace ComicSystem.Models
{
    public class RentalDetail
    {
        public int RentalDetailId { get; set; }
        public int RentalId { get; set; }
        public int ComicId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerDay { get; set; }

        public Rental Rental { get; set; }
        public Comic Comic { get; set; }
    }
}