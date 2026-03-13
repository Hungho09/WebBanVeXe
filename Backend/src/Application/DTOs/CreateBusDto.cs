using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs
{
    public class CreateBusDto
    {
        [Required]
        [MaxLength(50)]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        public BusType BusType { get; set; }

        [Required]
        [Range(10, 60, ErrorMessage = "Seat capacity must be between 10 and 60")]
        public int SeatCapacity { get; set; }
    }
}
