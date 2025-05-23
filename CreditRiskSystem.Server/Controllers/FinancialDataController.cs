using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CreditRiskSystem.Common.Models;
using CreditRiskSystem.Server.Data;

namespace CreditRiskSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FinancialDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FinancialData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinancialData>>> GetFinancialData()
        {
            return await _context.FinancialData.ToListAsync();
        }

        // GET: api/FinancialData/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FinancialData>> GetFinancialData(Guid id)
        {
            var financialData = await _context.FinancialData.FindAsync(id);

            if (financialData == null)
            {
                return NotFound();
            }

            return financialData;
        }

        // PUT: api/FinancialData/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFinancialData(Guid id, FinancialData financialData)
        {
            if (id != financialData.Id)
            {
                return BadRequest();
            }

            _context.Entry(financialData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinancialDataExists(id))
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

        // POST: api/FinancialData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FinancialData>> PostFinancialData(FinancialData financialData)
        {
            _context.FinancialData.Add(financialData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFinancialData", new { id = financialData.Id }, financialData);
        }

        // DELETE: api/FinancialData/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFinancialData(Guid id)
        {
            var financialData = await _context.FinancialData.FindAsync(id);
            if (financialData == null)
            {
                return NotFound();
            }

            _context.FinancialData.Remove(financialData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FinancialDataExists(Guid id)
        {
            return _context.FinancialData.Any(e => e.Id == id);
        }
    }
}
