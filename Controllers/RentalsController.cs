using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComicSystem.Models;
using ComicSystem.Data;

namespace ComicSystem.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ComicSystemContext _context;

        public RentalsController(ComicSystemContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rentals = _context.Rentals.ToList();
            return View(rentals);
        }

        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var rental = _context.Rentals.Find(id);
            if (rental == null)
            {
                return NotFound();
            }
            return View(rental);
        }
        [HttpPost]
        public IActionResult Create(int customerId, int comicId, int quantity, DateTime returnDate)
        {
            var rental = new Rental
            {
                CustomerId = customerId,
                RentalDate = DateTime.Now,
                ReturnDate = returnDate
            };
            _context.Rentals.Add(rental);
            _context.SaveChanges();

            var detail = new RentalDetail
            {
                RentalId = rental.RentalId,
                ComicId = comicId,
                Quantity = quantity,
                PricePerDay = 5000
            };
            _context.RentalDetails.Add(detail);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }

}