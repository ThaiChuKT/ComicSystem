using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComicSystem.Models;
using ComicSystem.Data;

namespace ComicSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComicController : ControllerBase
    {
        private readonly ComicSystemContext _context;

        public ComicController(ComicSystemContext context)
        {
            _context = context;
        }

        // GET: api/Comic
        [HttpGet]
        public async Task<IActionResult> GetComics()
        {
            var comics = await _context.Comics.ToListAsync();
            return Ok(comics);
        }

        // GET: api/Comic/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComic(int id)
        {
            var comic = await _context.Comics.FindAsync(id);

            if (comic == null)
            {
                return NotFound();
            }

            return Ok(comic);
        }

        // POST: api/Comic
        [HttpPost]
        public async Task<IActionResult> PostComic(Comic comic)
        {
            _context.Comics.Add(comic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComic), new { id = comic.ComicId }, comic);
        }

        // PUT: api/Comic/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComic(int id, Comic comic)
        {
            if (id != comic.ComicId)
            {
                return BadRequest();
            }

            _context.Entry(comic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Comic/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComic(int id)
        {
            var comic = await _context.Comics.FindAsync(id);
            if (comic == null)
            {
                return NotFound();
            }

            _context.Comics.Remove(comic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ComicExists(int id)
        {
            return _context.Comics.Any(e => e.ComicId == id);
        }
    }
}