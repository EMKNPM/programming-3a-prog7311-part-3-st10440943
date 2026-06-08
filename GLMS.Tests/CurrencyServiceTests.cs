using GLMS.Web.Services;
using System.Net.Http;
using Xunit;

namespace GLMS.Tests
{
    public class CurrencyServiceTests
    {
        private CurrencyService GetService()
        {
            var httpClient = new HttpClient();
            return new CurrencyService(httpClient);
        }

        [Fact]
        public void ConvertUsdToZar_CorrectMath()
        {
            var service = GetService();
            decimal result = service.ConvertUsdToZar(100m, 18.50m);
            Assert.Equal(1850.00m, result);
        }

        [Fact]
        public void ConvertUsdToZar_ZeroAmount_ReturnsZero()
        {
            var service = GetService();
            decimal result = service.ConvertUsdToZar(0m, 18.50m);
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ConvertUsdToZar_RoundsToTwoDecimals()
        {
            var service = GetService();
            decimal result = service.ConvertUsdToZar(1m, 18.555m);
            Assert.Equal(18.56m, result);
        }

        [Fact]
        public void ConvertUsdToZar_LargeAmount_CorrectResult()
        {
            var service = GetService();
            decimal result = service.ConvertUsdToZar(10000m, 18.50m);
            Assert.Equal(185000.00m, result);
        }
    }
}