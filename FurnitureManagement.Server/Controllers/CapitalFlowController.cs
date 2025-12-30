using FurnitureManagement.Server.Data;
using FurnitureManagement.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurnitureManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CapitalFlowController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CapitalFlowController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CapitalFlow
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CapitalFlow>>> GetCapitalFlows()
        {
            return await _context.CapitalFlow.ToListAsync();
        }

        // GET: api/CapitalFlow/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CapitalFlow>> GetCapitalFlow(int id)
        {
            var capitalFlow = await _context.CapitalFlow.FindAsync(id);

            if (capitalFlow == null)
            {
                return NotFound();
            }

            return capitalFlow;
        }

        // POST: api/CapitalFlow
        [HttpPost]
        public async Task<ActionResult<CapitalFlow>> PostCapitalFlow(CapitalFlow capitalFlow)
        {
            _context.CapitalFlow.Add(capitalFlow);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCapitalFlow), new { id = capitalFlow.FlowId }, capitalFlow);
        }

        // PUT: api/CapitalFlow/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCapitalFlow(int id, CapitalFlow capitalFlow)
        {
            if (id != capitalFlow.FlowId)
            {
                return BadRequest();
            }

            _context.Entry(capitalFlow).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CapitalFlowExists(id))
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

        // DELETE: api/CapitalFlow/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCapitalFlow(int id)
        {
            var capitalFlow = await _context.CapitalFlow.FindAsync(id);
            if (capitalFlow == null)
            {
                return NotFound();
            }

            _context.CapitalFlow.Remove(capitalFlow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/CapitalFlow/Summary
        [HttpGet("Summary")]
        public async Task<ActionResult<object>> GetCapitalFlowSummary()
        {
            var totalIncome = await _context.CapitalFlow
                .Where(cf => cf.FlowType == "income")
                .SumAsync(cf => cf.Amount);

            var totalExpense = await _context.CapitalFlow
                .Where(cf => cf.FlowType == "expense")
                .SumAsync(cf => cf.Amount);

            var balance = totalIncome - totalExpense;

            return new {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = balance
            };
        }

        private bool CapitalFlowExists(int id)
        {
            return _context.CapitalFlow.Any(e => e.FlowId == id);
        }
    }
}