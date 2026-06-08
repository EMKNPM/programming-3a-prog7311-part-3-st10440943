using GLMS.Web.Data;
using GLMS.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Web.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly AppDbContext _context;

        public ServiceRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceRequest>> GetAllAsync() =>
            await _context.ServiceRequests.Include(sr => sr.Contract).ToListAsync();

        public async Task<ServiceRequest?> GetByIdAsync(int id) =>
            await _context.ServiceRequests.Include(sr => sr.Contract)
                                          .FirstOrDefaultAsync(sr => sr.Id == id);

        public async Task<(bool Success, string Error)> CreateAsync(ServiceRequest request)
        {
            // *** CORE BUSINESS RULE ***
            var contract = await _context.Contracts.FindAsync(request.ContractId);

            if (contract == null)
                return (false, "Contract not found.");

            if (contract.Status == ContractStatus.Expired)
                return (false, "Cannot create a service request against an Expired contract.");

            if (contract.Status == ContractStatus.OnHold)
                return (false, "Cannot create a service request against a contract that is On Hold.");

            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task DeleteAsync(int id)
        {
            var sr = await _context.ServiceRequests.FindAsync(id);
            if (sr != null)
            {
                _context.ServiceRequests.Remove(sr);
                await _context.SaveChangesAsync();
            }
        }
    }
}