using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.Models
{
    public enum RequestStatus { Pending, InProgress, Completed, Cancelled }

    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }

        [Required, StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Amount entered by user in USD
        [Required, Range(0.01, double.MaxValue)]
        public decimal CostUSD { get; set; }

        // Auto-calculated from API
        public decimal CostZAR { get; set; }

        public decimal ExchangeRateUsed { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}