using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using dca_cal.Data;

namespace dca_cal.Data
{
	public class Crypto
    {
        [Key]
        public Guid ID { get; set; }
        public CryptoType Type { get; set; } = CryptoType.BTC;
        public decimal value { get; set; }
        public DateTime Date { get; set; }
    }
}


public enum CryptoType
{
    [Description("BTC")]
    BTC,    //Bitcoin
    [Description("ETH")]
    ETH,    //Ethereum
    [Description("SOL")]
    SOL,   
    [Description("DOGE")]
    DOGE,
    [Description("TON")]
    TON,
    [Description("TRON")]
    TRON,
}
