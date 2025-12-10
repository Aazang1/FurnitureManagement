using FurnitureManagement.Server.Data;
using FurnitureManagement.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FurnitureController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FurnitureController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Furniture
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Furniture>>> GetFurniture()
        {
            return await _context.Furniture.ToListAsync();
        }

        // GET: api/Furniture/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Furniture>> GetFurniture(int id)
        {
            var furniture = await _context.Furniture.FindAsync(id);

            if (furniture == null)
            {
                return NotFound();
            }

            return furniture;
        }

        // POST: api/Furniture
        [HttpPost]
        public async Task<ActionResult<Furniture>> CreateFurniture([FromBody] Furniture furniture)
        {
            _context.Furniture.Add(furniture);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFurniture), new { id = furniture.FurnitureId }, furniture);
        }

        // PUT: api/Furniture/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFurniture(int id, [FromBody] Furniture furniture)
        {
            if (id != furniture.FurnitureId)
            {
                return BadRequest();
            }

            _context.Entry(furniture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FurnitureExists(id))
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

        // DELETE: api/Furniture/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFurniture(int id)
        {
            var furniture = await _context.Furniture.FindAsync(id);
            if (furniture == null)
            {
                return NotFound();
            }

            _context.Furniture.Remove(furniture);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FurnitureExists(int id)
        {
            return _context.Furniture.Any(e => e.FurnitureId == id);
        }
    }
}