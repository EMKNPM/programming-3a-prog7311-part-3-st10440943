using GLMS.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GLMS.Web.ApiServices
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient("GLMSAPI");
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private StringContent ToJson(object obj) =>
            new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

        // ── AUTH ──
        public async Task<string?> LoginAsync(string username, string password)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("GLMSAPI");
                var response = await client.PostAsync("api/auth/login",
                    ToJson(new { username, password }));
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("token").GetString();
            }
            catch { return null; }
        }

        // ── CLIENTS ──
        public async Task<List<Client>> GetClientsAsync()
        {
            try
            {
                var response = await GetClient().GetAsync("api/clients");
                if (!response.IsSuccessStatusCode) return new List<Client>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Client>>(json, _jsonOptions) ?? new List<Client>();
            }
            catch { return new List<Client>(); }
        }

        public async Task<Client?> GetClientAsync(int id)
        {
            try
            {
                var response = await GetClient().GetAsync($"api/clients/{id}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Client>(json, _jsonOptions);
            }
            catch { return null; }
        }

        public async Task<bool> CreateClientAsync(Client client)
        {
            try
            {
                var response = await GetClient().PostAsync("api/clients", ToJson(client));
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> UpdateClientAsync(Client client)
        {
            try
            {
                var response = await GetClient().PutAsync($"api/clients/{client.Id}", ToJson(client));
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            try
            {
                var response = await GetClient().DeleteAsync($"api/clients/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // ── CONTRACTS ──
        public async Task<List<Contract>> GetContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            try
            {
                var url = "api/contracts?";
                if (startDate.HasValue) url += $"startDate={startDate:yyyy-MM-dd}&";
                if (endDate.HasValue) url += $"endDate={endDate:yyyy-MM-dd}&";
                if (status.HasValue) url += $"status={status}";
                var response = await GetClient().GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<Contract>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Contract>>(json, _jsonOptions) ?? new List<Contract>();
            }
            catch { return new List<Contract>(); }
        }

        public async Task<Contract?> GetContractAsync(int id)
        {
            try
            {
                var response = await GetClient().GetAsync($"api/contracts/{id}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Contract>(json, _jsonOptions);
            }
            catch { return null; }
        }

        public async Task<bool> CreateContractAsync(Contract contract)
        {
            try
            {
                var response = await GetClient().PostAsync("api/contracts", ToJson(contract));
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> UpdateContractAsync(Contract contract)
        {
            try
            {
                var response = await GetClient().PutAsync($"api/contracts/{contract.Id}", ToJson(contract));
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> UpdateContractStatusAsync(int id, ContractStatus status)
        {
            try
            {
                var response = await GetClient().PatchAsync(
                    $"api/contracts/{id}/status", ToJson(status));
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            try
            {
                var response = await GetClient().DeleteAsync($"api/contracts/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // ── SERVICE REQUESTS ──
        public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
        {
            try
            {
                var response = await GetClient().GetAsync("api/servicerequests");
                if (!response.IsSuccessStatusCode) return new List<ServiceRequest>();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ServiceRequest>>(json, _jsonOptions) ?? new List<ServiceRequest>();
            }
            catch { return new List<ServiceRequest>(); }
        }

        public async Task<ServiceRequest?> GetServiceRequestAsync(int id)
        {
            try
            {
                var response = await GetClient().GetAsync($"api/servicerequests/{id}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ServiceRequest>(json, _jsonOptions);
            }
            catch { return null; }
        }

        public async Task<(bool Success, string Error)> CreateServiceRequestAsync(ServiceRequest request)
        {
            try
            {
                var response = await GetClient().PostAsync("api/servicerequests", ToJson(request));
                if (response.IsSuccessStatusCode) return (true, string.Empty);
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var error = doc.RootElement.GetProperty("message").GetString() ?? "Unknown error";
                return (false, error);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            try
            {
                var response = await GetClient().DeleteAsync($"api/servicerequests/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // ── CURRENCY ──
        public async Task<decimal> GetExchangeRateAsync()
        {
            try
            {
                var response = await GetClient().GetAsync("api/servicerequests/rate");
                if (!response.IsSuccessStatusCode) return 18.50m;
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("rate").GetDecimal();
            }
            catch { return 18.50m; }
        }
    }
}