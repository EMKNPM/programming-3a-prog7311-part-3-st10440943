using GLMS.Web.Models;

namespace GLMS.Web.Services
{
    public interface IContractService
    {
        Task<List<Contract>> GetAllAsync();
        Task<List<Contract>> SearchAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
        Task<Contract?> GetByIdAsync(int id);
        Task CreateAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);
    }
}