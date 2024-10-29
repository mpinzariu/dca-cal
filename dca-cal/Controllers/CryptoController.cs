using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dca_cal.Data;

namespace dca_cal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CryptoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Crypto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Crypto>>> GetCryptos()
        {
            return await _context.Cryptos.ToListAsync();
        }

        // GET: api/Crypto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Crypto>> GetCrypto(Guid id)
        {
            var crypto = await _context.Cryptos.FindAsync(id);

            if (crypto == null)
            {
                return NotFound();
            }

            return crypto;
        }

        // PUT: api/Crypto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCrypto(Guid id, Crypto crypto)
        {
            if (id != crypto.ID)
            {
                return BadRequest();
            }

            _context.Entry(crypto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CryptoExists(id))
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

        // POST: api/Crypto
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Crypto>> PostCrypto(Crypto crypto)
        {
            _context.Cryptos.Add(crypto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCrypto", new { id = crypto.ID }, crypto);
        }

        // DELETE: api/Crypto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrypto(Guid id)
        {
            var crypto = await _context.Cryptos.FindAsync(id);
            if (crypto == null)
            {
                return NotFound();
            }

            _context.Cryptos.Remove(crypto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CryptoExists(Guid id)
        {
            return _context.Cryptos.Any(e => e.ID == id);
        }
    }
}
