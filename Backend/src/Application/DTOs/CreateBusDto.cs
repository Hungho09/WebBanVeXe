using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs
{
    public class CreateBusDto
    {
        [Required]
        [MaxLength(50)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        public Guid BusTypeId { get; set; }

        public BusStatus Status { get; set; } = BusStatus.Available;
    }
}
