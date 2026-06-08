using GLMS.Web.Models;

namespace GLMS.Web.ApiServices
{
    public interface IApiService
    {
        // Auth
        Task<string?> LoginAsync(string username, string password);

        // Clients
        Task<List<Client>> GetClientsAsync();
        Task<Client?> GetClientAsync(int id);
        Task<bool> CreateClientAsync(Client client);
        Task<bool> UpdateClientAsync(Client client);
        Task<bool> DeleteClientAsync(int id);

        // Contracts
        Task<List<Contract>> GetContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
        Task<Contract?> GetContractAsync(int id);
        Task<bool> CreateContractAsync(Contract contract);
        Task<bool> UpdateContractAsync(Contract contract);
        Task<bool> UpdateContractStatusAsync(int id, ContractStatus status);
        Task<bool> DeleteContractAsync(int id);

        // Service Requests
        Task<List<ServiceRequest>> GetServiceRequestsAsync();
        Task<ServiceRequest?> GetServiceRequestAsync(int id);
        Task<(bool Success, string Error)> CreateServiceRequestAsync(ServiceRequest request);
        Task<bool> DeleteServiceRequestAsync(int id);

        // Currency
        Task<decimal> GetExchangeRateAsync();
    }
}