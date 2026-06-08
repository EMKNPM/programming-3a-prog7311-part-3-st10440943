using GLMS.Web.Models;
using GLMS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _srService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            IServiceRequestService srService,
            ICurrencyService currencyService)
        {
            _srService = srService;
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _srService.GetAllAsync();
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _srService.GetByIdAsync(id);
            if (request == null) return NotFound(new { message = "Service request not found" });
            return Ok(request);
        }

        [HttpGet("rate")]
        [AllowAnonymous]
        public async Task<IActionResult> GetExchangeRate()
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();
            return Ok(new
            {
                baseCurrency = "USD",
                targetCurrency = "ZAR",
                rate = rate,
                retrievedAt = DateTime.UtcNow
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var rate = await _currencyService.GetUsdToZarRateAsync();
            request.CostZAR = _currencyService.ConvertUsdToZar(request.CostUSD, rate);
            request.ExchangeRateUsed = rate;
            var (success, error) = await _srService.CreateAsync(request);
            if (!success) return BadRequest(new { message = error });
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _srService.GetByIdAsync(id);
            if (request == null) return NotFound(new { message = "Service request not found" });
            await _srService.DeleteAsync(id);
            return Ok(new { message = "Service request deleted" });
        }
    }
}