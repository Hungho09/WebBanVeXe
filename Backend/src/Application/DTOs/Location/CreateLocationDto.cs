using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Location
{
    public class CreateLocationDto
    {
        [Required(ErrorMessage = "Tên địa điểm không được để trống")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; } = string.Empty;
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDropoff { get; set; }
        public string? Badge { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
        public Guid ProvinceId { get; set; }
        
        public string? MapLink { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
