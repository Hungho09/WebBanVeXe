using System;

namespace Domain.Entities
{
    public class CmsConfig
    {
        public int Id { get; set; }
        public string ConfigKey { get; set; } = string.Empty; // e.g., "homepage_v1"
        public string ContentJson { get; set; } = string.Empty; // JSON string of the CMS configuration
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
