using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Province
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Region { get; set; } = string.Empty; // MienBac, MienTrung, MienNam
        
        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}
