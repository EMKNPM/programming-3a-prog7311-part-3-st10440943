using GLMS.Web.ApiServices;
using GLMS.Web.Models;
using GLMS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IFileService _fileService;

        public ContractsController(IApiService apiService, IFileService fileService)
        {
            _apiService = apiService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index(
            DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = await _apiService.GetContractsAsync(startDate, endDate, status);
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;
            ViewBag.Statuses = Enum.GetValues(typeof(ContractStatus));
            return View(contracts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        public async Task<IActionResult> Create()
        {
            var clients = await _apiService.GetClientsAsync();
            ViewBag.Clients = clients;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? signedAgreement)
        {
            if (signedAgreement != null)
            {
                if (!_fileService.IsValidPdf(signedAgreement))
                {
                    ModelState.AddModelError("signedAgreement", "Only PDF files allowed.");
                    ViewBag.Clients = await _apiService.GetClientsAsync();
                    return View(contract);
                }
                contract.SignedAgreementPath = await _fileService.SaveContractFileAsync(signedAgreement);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Clients = await _apiService.GetClientsAsync();
                return View(contract);
            }

            await _apiService.CreateContractAsync(contract);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            ViewBag.Clients = await _apiService.GetClientsAsync();
            return View(contract);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Contract contract, IFormFile? signedAgreement)
        {
            if (signedAgreement != null)
            {
                if (!_fileService.IsValidPdf(signedAgreement))
                {
                    ModelState.AddModelError("signedAgreement", "Only PDF files allowed.");
                    ViewBag.Clients = await _apiService.GetClientsAsync();
                    return View(contract);
                }
                contract.SignedAgreementPath = await _fileService.SaveContractFileAsync(signedAgreement);
            }

            await _apiService.UpdateContractAsync(contract);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Download(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return NotFound();
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
            if (!System.IO.File.Exists(fullPath)) return NotFound();
            return PhysicalFile(fullPath, "application/pdf", Path.GetFileName(fullPath));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteContractAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}