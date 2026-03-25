using System;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Cms;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CmsService : ICmsService
    {
        private readonly IApplicationDbContext _context;

        public CmsService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CmsConfigDto?> GetConfigAsync(string key)
        {
            var config = await _context.CmsConfigs
                .FirstOrDefaultAsync(x => x.ConfigKey == key);

            if (config == null) return null;

            return new CmsConfigDto
            {
                ConfigKey = config.ConfigKey,
                ContentJson = config.ContentJson
            };
        }

        public async Task<bool> UpdateConfigAsync(CmsConfigDto dto)
        {
            var config = await _context.CmsConfigs
                .FirstOrDefaultAsync(x => x.ConfigKey == dto.ConfigKey);

            if (config == null)
            {
                // Create if not exists
                config = new CmsConfig
                {
                    ConfigKey = dto.ConfigKey,
                    ContentJson = dto.ContentJson,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CmsConfigs.Add(config);
            }
            else
            {
                // Update
                config.ContentJson = dto.ContentJson;
                config.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
