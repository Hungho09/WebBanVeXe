using System;

namespace Application.DTOs.Location
{
    public class ProvinceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }
}
