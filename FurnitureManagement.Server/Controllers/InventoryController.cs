using FurnitureManagement.Server.Data;
using FurnitureManagement.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventory()
        {
            return await _context.Inventory
                .Include(i => i.Furniture)
                .Include(i => i.Warehouse)
                .ToListAsync();
        }

        // GET: api/Inventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var inventory = await _context.Inventory
                .Include(i => i.Furniture)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(i => i.InventoryId == id);

            if (inventory == null)
            {
                return NotFound();
            }

            return inventory;
        }

        // GET: api/Inventory/by-furniture/5
        [HttpGet("by-furniture/{furnitureId}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryByFurnitureId(int furnitureId)
        {
            return await _context.Inventory
                .Where(i => i.FurnitureId == furnitureId)
                .Include(i => i.Warehouse)
                .ToListAsync();
        }

        // GET: api/Inventory/by-warehouse/5
        [HttpGet("by-warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryByWarehouseId(int warehouseId)
        {
            return await _context.Inventory
                .Where(i => i.WarehouseId == warehouseId)
                .Include(i => i.Furniture)
                .ToListAsync();
        }

        // POST: api/Inventory
        [HttpPost]
        public async Task<ActionResult<Inventory>> CreateInventory([FromBody] Inventory inventory)
        {
            // 设置最后更新时间
            inventory.LastUpdated = DateTime.Now;
            
            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            // 返回包含关联数据的结果
            return await _context.Inventory
                .Include(i => i.Furniture)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(i => i.InventoryId == inventory.InventoryId);
        }

        // PUT: api/Inventory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] Inventory inventory)
        {
            if (id != inventory.InventoryId)
            {
                return BadRequest();
            }

            // 更新最后更新时间
            inventory.LastUpdated = DateTime.Now;
            
            _context.Entry(inventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(id))
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

        // PATCH: api/Inventory/5/update-quantity
        [HttpPatch("{id}/update-quantity")]
        public async Task<IActionResult> UpdateInventoryQuantity(int id, [FromBody] int quantity)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            inventory.Quantity = quantity;
            inventory.LastUpdated = DateTime.Now;
            
            _context.Entry(inventory).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            _context.Inventory.Remove(inventory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventory.Any(e => e.InventoryId == id);
        }
    }
}