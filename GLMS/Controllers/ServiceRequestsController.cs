using GLMS.Web.ApiServices;
using GLMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Web.Controllers
{
    [Authorize]
    public class ServiceRequestsController : Controller
    {
        private readonly IApiService _apiService;

        public ServiceRequestsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _apiService.GetServiceRequestsAsync();
            return View(requests);
        }

        public async Task<IActionResult> Create()
        {
            var rate = await _apiService.GetExchangeRateAsync();
            ViewBag.ExchangeRate = rate;
            var contracts = await _apiService.GetContractsAsync(null, null, ContractStatus.Active);
            ViewBag.Contracts = new SelectList(contracts, "Id", "ServiceLevel");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest request)
        {
            var rate = await _apiService.GetExchangeRateAsync();

            if (!ModelState.IsValid)
            {
                ViewBag.ExchangeRate = rate;
                var contracts = await _apiService.GetContractsAsync(null, null, ContractStatus.Active);
                ViewBag.Contracts = new SelectList(contracts, "Id", "ServiceLevel");
                return View(request);
            }

            var (success, error) = await _apiService.CreateServiceRequestAsync(request);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                ViewBag.ExchangeRate = rate;
                var contracts = await _apiService.GetContractsAsync(null, null, ContractStatus.Active);
                ViewBag.Contracts = new SelectList(contracts, "Id", "ServiceLevel");
                return View(request);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var sr = await _apiService.GetServiceRequestAsync(id);
            if (sr == null) return NotFound();
            return View(sr);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteServiceRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}