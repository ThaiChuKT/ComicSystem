using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComicSystem.Models;
using ComicSystem.Data;
using System.Transactions;

namespace ComicSystem.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ComicSystemContext _context;

        public RentalsController(ComicSystemContext context)
        {
            _context = context;
        }

        // GET: Rentals
        public async Task<IActionResult> Index()
        {
            var rentals = await _context.Rentals.Include(r => r.Customer).ToListAsync();
            return View(rentals);
        }

        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.RentalDetails)
                .ThenInclude(rd => rd.Comic)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: Rentals/Create
        public IActionResult Create()
        {
            ViewData["Customers"] = _context.Customers.ToList();
            ViewData["Comics"] = _context.Comics.ToList();
            return View();
        }

        // POST: Rentals/Create
        [HttpPost]
        public async Task<IActionResult> Create(int customerId, int comicId, int quantity, DateTime returnDate)
        {
            if (returnDate <= DateTime.Now)
            {
                ModelState.AddModelError("", "Return date must be in the future.");
                ViewData["Customers"] = _context.Customers.ToList();
                ViewData["Comics"] = _context.Comics.ToList();
                return View();
            }

            var comic = await _context.Comics.FindAsync(comicId);
            if (comic == null || quantity <= 0 || quantity > comic.Stock)
            {
                ModelState.AddModelError("", "Invalid comic or insufficient stock.");
                ViewData["Customers"] = _context.Customers.ToList();
                ViewData["Comics"] = _context.Comics.ToList();
                return View();
            }

            if (!decimal.TryParse(comic.Price, out decimal pricePerDay))
            {
                ModelState.AddModelError("", "Invalid price for comic.");
                ViewData["Customers"] = _context.Customers.ToList();
                ViewData["Comics"] = _context.Comics.ToList();
                return View();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var rental = new Rental
                    {
                        CustomerId = customerId,
                        RentalDate = DateTime.Now,
                        ReturnDate = returnDate
                    };
                    _context.Rentals.Add(rental);
                    await _context.SaveChangesAsync();

                    var detail = new RentalDetail
                    {
                        RentalId = rental.RentalId,
                        ComicId = comicId,
                        Quantity = quantity,
                        PricePerDay = pricePerDay
                    };
                    _context.RentalDetails.Add(detail);

                    comic.Stock -= quantity;
                    _context.Comics.Update(comic);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "An error occurred while creating the rental.");
                    ViewData["Customers"] = _context.Customers.ToList();
                    ViewData["Comics"] = _context.Comics.ToList();
                    return View();
                }
            }
        }

        // GET: Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            return View(rental);
        }

        // POST: Rentals/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,CustomerId,RentalDate,ReturnDate")] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rental);
        }

        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.RentalId == id);
        }
    }
}