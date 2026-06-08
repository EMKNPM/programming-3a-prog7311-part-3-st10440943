using GLMS.Web.Models;

namespace GLMS.Web.Services
{
    public interface IServiceRequestService
    {
        Task<List<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<(bool Success, string Error)> CreateAsync(ServiceRequest request);
        Task DeleteAsync(int id);
    }
}