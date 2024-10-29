using Azure;
using dca_cal.Data;

namespace dca_cal.Services
{
    public class InvestmentService
    {
        private readonly HttpClient _httpClient;

        public InvestmentService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://localhost:7277/");
            _httpClient = httpClient;
        }

        public async Task<List<Investment>> CalculateDCA(CryptoType cryptoType, DateTime startDate, decimal monthlyInvestment, string frequency)
        {
            var response = await _httpClient.GetAsync($"api/Investment/CalculateDCA?cryptoType={cryptoType}&startDate={startDate}&monthlyInvestment={monthlyInvestment}&frequency={frequency}");

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response into a List<WeatherForecast>
                return await response.Content.ReadFromJsonAsync<List<Investment>>();
            }

            return new List<Investment>();
        }
    }
}