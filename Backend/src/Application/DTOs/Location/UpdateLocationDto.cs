using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Location
{
    public class UpdateLocationDto
    {
        [Required(ErrorMessage = "Tên địa điểm không được để trống")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; } = string.Empty;
        
        public string ProvinceName { get; set; } = string.Empty;
        
        public string? MapLink { get; set; }
    }
}
