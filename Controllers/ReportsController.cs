using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComicSystem.Models;
using ComicSystem.Data;

namespace ComicSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ComicSystemContext _context;

        public ReportsController(ComicSystemContext context)
        {
            _context = context;
        }

        // GET: Reports
        public IActionResult Index(DateTime startDate, DateTime endDate)
        {
            var report = from r in _context.Rentals
                         join rd in _context.RentalDetails on r.RentalId equals rd.RentalId
                         join cb in _context.Comics on rd.ComicId equals cb.ComicId
                         join c in _context.Customers on r.CustomerId equals c.CustomerId
                         where r.RentalDate >= startDate && r.RentalDate <= endDate
                         select new
                         {
                             r.RentalId,
                             r.CustomerId,
                             r.RentalDate,
                             r.ReturnDate,
                             cb.Title,
                             c.Name,
                             rd.Quantity,
                             rd.PricePerDay,
                             TotalPrice = rd.Quantity * rd.PricePerDay * EF.Functions.DateDiffDay(r.RentalDate, r.ReturnDate)
                         };
            return View(report.ToList());
        }
    }
}