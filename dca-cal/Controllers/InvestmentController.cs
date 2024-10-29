using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dca_cal.Data;
using dca_cal.Services;

namespace dca_cal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CoinMarketCapService _coinMarketCapService;

        public InvestmentController(AppDbContext context, CoinMarketCapService coinMarketCapService)
        {
            _context = context;
            _coinMarketCapService = coinMarketCapService;
        }

        // GET: api/Investment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Investment>>> GetInvestments()
        {
            return await _context.Investments.ToListAsync();
        }

        [HttpGet]
        [Route("CalculateDCA")]
        public async Task<ActionResult<IEnumerable<Investment>>> CalculateDCA(CryptoType cryptoType, DateTime startDate, decimal monthlyInvestment, string frequency)
        {
            var investmentRecords = new List<Investment>();
            DateTime currentDate = startDate;
            decimal totalInvested = 0;
            decimal totalCryptoAmount = 0;

            //decimal currentPrice = await _coinMarketCapService.GetHistoricalPrice(cryptoType.ToString(), DateTime.Today, null);
            decimal currentPrice = await GetCryptoPrice(cryptoType, DateTime.Today);

            while (currentDate <= DateTime.Today)
            {
                decimal historicalPrice = await GetCryptoPrice(cryptoType, currentDate);

                decimal cryptoBought = monthlyInvestment / historicalPrice;
                totalInvested += monthlyInvestment;
                totalCryptoAmount += cryptoBought;

                decimal valueToday = totalCryptoAmount * currentPrice;
                decimal roi = (valueToday - totalInvested) / totalInvested * 100;

                investmentRecords.Add(new Investment
                {
                    ID = Guid.NewGuid(),
                    Date = currentDate,
                    InvestedAmount = monthlyInvestment,
                    CryptoAmount = cryptoBought,
                    CryptoType = cryptoType,
                    ValueToday = valueToday,
                    ROI = roi
                });

                switch(frequency)
                {
                    case "15th":
                        currentDate = currentDate.AddDays(15);
                        break;
                    case "20th":
                        currentDate = currentDate.AddDays(20);
                        break;
                    case "25th":
                        currentDate = currentDate.AddDays(25);
                        break;
                    default:
                        currentDate = currentDate.AddMonths(1);
                        break;
                }
            }

            return Ok(investmentRecords);
        }

        private async Task<decimal> GetCryptoPrice(CryptoType cryptoType, DateTime currentDate)
        {
            var valCrypto = _context.Cryptos.Where(c => c.Type == cryptoType && c.Date.Day == currentDate.Day && c.Date.Month == currentDate.Month && c.Date.Year == currentDate.Year);
            decimal historicalPrice = 0;
            if (valCrypto.Count() > 0)
            {
                historicalPrice = valCrypto.First().value;
            }
            else
            {
                var today = DateTime.Now;
                if (currentDate.Day == today.Day && currentDate.Month == today.Month && currentDate.Year == today.Year)
                {
                    historicalPrice = await _coinMarketCapService.GetHistoricalPrice(cryptoType.ToString(), currentDate, null);
                }
                else
                {
                    historicalPrice = await _coinMarketCapService.GetHistoricalPrice(cryptoType.ToString(), currentDate, currentDate.AddDays(1));
                }
                
                _context.Cryptos.Add(new Crypto
                {
                    Date = currentDate,
                    Type = cryptoType,
                    value = historicalPrice,
                });
                _context.SaveChanges();
            }

            return historicalPrice;
        }

        // GET: api/Investment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Investment>> GetInvestment(Guid id)
        {
            var investment = await _context.Investments.FindAsync(id);

            if (investment == null)
            {
                return NotFound();
            }

            return investment;
        }

        // PUT: api/Investment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvestment(Guid id, Investment investment)
        {
            if (id != investment.ID)
            {
                return BadRequest();
            }

            _context.Entry(investment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvestmentExists(id))
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

        // POST: api/Investment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Investment>> PostInvestment(Investment investment)
        {
            _context.Investments.Add(investment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvestment", new { id = investment.ID }, investment);
        }

        // DELETE: api/Investment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment(Guid id)
        {
            var investment = await _context.Investments.FindAsync(id);
            if (investment == null)
            {
                return NotFound();
            }

            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvestmentExists(Guid id)
        {
            return _context.Investments.Any(e => e.ID == id);
        }
    }
}
