using System;
using Microsoft.EntityFrameworkCore;

namespace dca_cal.Data
{
	public class AppDbContext : DbContext
	{
		public DbSet<Investment> Investments { get; set; }
        public DbSet<Crypto> Cryptos { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}
	}
}

