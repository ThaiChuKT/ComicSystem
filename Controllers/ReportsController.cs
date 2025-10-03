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
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue || startDate > endDate)
            {
                ViewBag.Error = "Please provide valid start and end dates, with start date before end date.";
                return View(new List<object>());
            }

            var report = await (from r in _context.Rentals
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
                                    TotalPrice = rd.Quantity * rd.PricePerDay * Math.Max(1, EF.Functions.DateDiffDay(r.RentalDate, r.ReturnDate ?? DateTime.Now))
                                }).ToListAsync();

            return View(report);
        }
    }
}