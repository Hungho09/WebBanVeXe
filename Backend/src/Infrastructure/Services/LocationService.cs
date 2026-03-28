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
            var query = _context.Locations
                .Include(l => l.Province)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerTerm = searchTerm.ToLower();
                query = query.Where(l => l.Name.ToLower().Contains(lowerTerm) 
                                      || (l.Province != null && l.Province.Name.ToLower().Contains(lowerTerm)));
            }

            var locations = await query
                .OrderByDescending(l => l.IsDefault) // Ưu tiên điểm mặc định lên đầu
                .ThenBy(l => l.Province != null ? l.Province.Name : "")
                .ThenBy(l => l.Name)
                .ToListAsync();

            return locations.Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Address = l.Address,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                IsPickup = l.IsPickup,
                IsDropoff = l.IsDropoff,
                Badge = l.Badge,
                ProvinceId = l.ProvinceId,
                MapLink = l.MapLink,
                IsDefault = l.IsDefault,
                IsActive = l.IsActive,
                Province = l.Province != null ? new ProvinceDto
                {
                    Id = l.Province.Id,
                    Name = l.Province.Name,
                    Slug = l.Province.Slug,
                    Region = l.Province.Region
                } : null
            });
        }

        public async Task<LocationDto?> GetLocationByIdAsync(Guid id)
        {
            var l = await _context.Locations.Include(loc => loc.Province).FirstOrDefaultAsync(loc => loc.Id == id);
            if (l == null) return null;

            return new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Address = l.Address,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                IsPickup = l.IsPickup,
                IsDropoff = l.IsDropoff,
                Badge = l.Badge,
                ProvinceId = l.ProvinceId,
                MapLink = l.MapLink,
                IsDefault = l.IsDefault,
                IsActive = l.IsActive,
                Province = l.Province != null ? new ProvinceDto
                {
                    Id = l.Province.Id,
                    Name = l.Province.Name,
                    Slug = l.Province.Slug,
                    Region = l.Province.Region
                } : null
            };
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto)
        {
            var location = new Location
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Address = createDto.Address,
                Latitude = createDto.Latitude,
                Longitude = createDto.Longitude,
                IsPickup = createDto.IsPickup,
                IsDropoff = createDto.IsDropoff,
                Badge = createDto.Badge,
                ProvinceId = createDto.ProvinceId,
                MapLink = createDto.MapLink,
                IsDefault = createDto.IsDefault,
                IsActive = createDto.IsActive
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
            location.Latitude = updateDto.Latitude;
            location.Longitude = updateDto.Longitude;
            location.IsPickup = updateDto.IsPickup;
            location.IsDropoff = updateDto.IsDropoff;
            location.Badge = updateDto.Badge;
            location.ProvinceId = updateDto.ProvinceId;
            location.MapLink = updateDto.MapLink;
            location.IsDefault = updateDto.IsDefault;
            location.IsActive = updateDto.IsActive;

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

        public async Task<bool> ToggleLocationDefaultStatusAsync(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return false;

            location.IsDefault = !location.IsDefault;
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProvinceDto>> GetAllProvincesAsync()
        {
            var provinces = await _context.Provinces.ToListAsync();
            return provinces.Select(p => new ProvinceDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Region = p.Region
            });
        }
    }
}
