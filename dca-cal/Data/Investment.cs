using System;
using System.ComponentModel.DataAnnotations;

namespace dca_cal.Data
{
	public class Investment
	{
        [Key]
        public Guid ID { get; set; }
        public DateTime Date { get; set; }
        public decimal InvestedAmount { get; set; }
        public decimal CryptoAmount { get; set; }
        public CryptoType CryptoType { get; set; }
        public decimal ValueToday { get; set; }
        public decimal ROI { get; set; }
    }
}

