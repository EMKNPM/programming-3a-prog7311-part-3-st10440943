using System.Text.Json;

namespace GLMS.Web.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                // Free API - no key needed for this endpoint
                var response = await _httpClient.GetAsync(
                    "https://open.er-api.com/v6/latest/USD");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                var rate = doc.RootElement
                    .GetProperty("rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                return rate;
            }
            catch
            {
                // Fallback rate if API is down
                return 18.50m;
            }
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal rate)
        {
            return Math.Round(usdAmount * rate, 2);
        }
    }
}