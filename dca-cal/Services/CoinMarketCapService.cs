using RestSharp;
using Newtonsoft.Json;
using System.Security.Policy;

namespace dca_cal.Services
{
    public class CoinMarketCapService
	{
        private readonly string _apiKey = "4f0652fc-fb2e-4583-813d-8c32ecbac1ec";

        public async Task<decimal> GetHistoricalPrice(string cryptocurrency, DateTime dateStart, DateTime? dateEnd)
        {
            var url = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/historical?symbol={cryptocurrency}&time_start={dateStart.ToString("yyyy-MM-dd")}";
            if (dateEnd != null)
            {
                url += $"&time_end={dateEnd?.ToString("yyyy-MM-dd")}";
                url += "&count=1&interval=daily";
            }

            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddHeader("X-CMC_PRO_API_KEY", _apiKey);
            request.AddHeader("Accepts", "application/json");

            var response = await client.GetAsync(request);
            if (response.IsSuccessful)
            {
                var data = JsonConvert.DeserializeObject<PriceDataResponse>(response.Content);

                // Extract the price from the response
                if (data?.Data?.Quotes != null && data.Data.Quotes.Count > 0)
                {
                    return data.Data.Quotes[0].Quote.USD.Price;
                }
                else
                {
                    return 0;
                    //throw new Exception("No price data available for the specified date.");
                }
            }

            throw new Exception("Failed to fetch data from CoinMarketCap");
        }
    }

    public class PriceDataResponse
    {
        public Status Status { get; set; }
        public Data Data { get; set; }
    }

    public class Status
    {
        public DateTime Timestamp { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int IsActive { get; set; }
        public int IsFiat { get; set; }
        public List<QuoteData> Quotes { get; set; }
    }

    public class QuoteData
    {
        public DateTime Timestamp { get; set; }
        public QuoteDetail Quote { get; set; }
    }

    public class QuoteDetail
    {
        public CurrencyData USD { get; set; }
    }

    public class CurrencyData
    {
        public decimal Price { get; set; }
        public decimal PercentChange1h { get; set; }
        public decimal PercentChange24h { get; set; }
        public decimal PercentChange7d { get; set; }
        public decimal PercentChange30d { get; set; }
        public decimal Volume24h { get; set; }
        public decimal MarketCap { get; set; }
        public decimal TotalSupply { get; set; }
        public decimal CirculatingSupply { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
