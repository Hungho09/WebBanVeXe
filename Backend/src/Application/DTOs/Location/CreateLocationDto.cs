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
        
        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố")]
        public string ProvinceName { get; set; } = string.Empty;
        
        public string? MapLink { get; set; }
    }
}
