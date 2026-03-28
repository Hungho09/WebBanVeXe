using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Location;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class LocationService : ILocationService
    {
        private readonly IApplicationDbContext _context;

        public LocationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsByProvinceAsync(string? searchTerm = null)
        {
            var query = _context.Locations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerTerm = searchTerm.ToLower();
                query = query.Where(l => l.Name.ToLower().Contains(lowerTerm) 
                                      || (l.ProvinceName != null && l.ProvinceName.ToLower().Contains(lowerTerm)));
            }

            var locations = await query
                .OrderBy(l => l.ProvinceName != null ? l.ProvinceName : "")
                .ThenBy(l => l.Name)
                .ToListAsync();

            return locations.Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Address = l.Address,
                ProvinceName = l.ProvinceName,
                MapLink = l.MapLink
            });
        }

        public async Task<LocationDto?> GetLocationByIdAsync(Guid id)
        {
            var l = await _context.Locations.FirstOrDefaultAsync(loc => loc.Id == id);
            if (l == null) return null;

            return new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Address = l.Address,
                ProvinceName = l.ProvinceName,
                MapLink = l.MapLink
            };
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto)
        {
            var location = new Location
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Address = createDto.Address,
                ProvinceName = createDto.ProvinceName,
                MapLink = createDto.MapLink
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return await GetLocationByIdAsync(location.Id) ?? throw new Exception("Failed to retrieve created location");
        }

        public async Task<bool> UpdateLocationAsync(Guid id, UpdateLocationDto updateDto)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return false;

            location.Name = updateDto.Name;
            location.Address = updateDto.Address;
            location.ProvinceName = updateDto.ProvinceName;
            location.MapLink = updateDto.MapLink;

            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLocationAsync(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return false;

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
