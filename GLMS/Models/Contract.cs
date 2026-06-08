using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.Models
{
    public enum ContractStatus { Draft, Active, Expired, OnHold }

    public class Contract
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Required, StringLength(200)]
        public string ServiceLevel { get; set; } = string.Empty;

        // PDF file path stored on server
        public string? SignedAgreementPath { get; set; }

        // Navigation
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}