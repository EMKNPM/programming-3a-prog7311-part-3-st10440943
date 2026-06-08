using GLMS.API;
using GLMS.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace GLMS.Tests.Integration
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<ApiProgram>>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<ApiProgram> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        private async Task<string?> GetTokenAsync()
        {
            var loginData = new { username = "admin", password = "Admin@1234" };
            var content = new StringContent(
                JsonSerializer.Serialize(loginData),
                Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("token").GetString();
        }

        private async Task AuthenticateAsync()
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            var loginData = new { username = "admin", password = "Admin@1234" };
            var content = new StringContent(
                JsonSerializer.Serialize(loginData),
                Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);
            var json = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("token", json);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var loginData = new { username = "wrong", password = "wrong" };
            var content = new StringContent(
                JsonSerializer.Serialize(loginData),
                Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_WithValidToken_ReturnsOk()
        {
            await AuthenticateAsync();
            var response = await _client.GetAsync("/api/contracts");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_WithoutToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/contracts");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetClients_WithValidToken_ReturnsOk()
        {
            await AuthenticateAsync();
            var response = await _client.GetAsync("/api/clients");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetClients_WithoutToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/clients");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetExchangeRate_NoAuthRequired_ReturnsOk()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/servicerequests/rate");
            var json = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("rate", json);
        }

        [Fact]
        public async Task GetServiceRequests_WithValidToken_ReturnsOk()
        {
            await AuthenticateAsync();
            var response = await _client.GetAsync("/api/servicerequests");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequests_WithoutToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/servicerequests");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}