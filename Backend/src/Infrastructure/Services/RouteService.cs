using Application.DTOs.Route;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;

        public RouteService(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public async Task<RouteDto?> GetRouteByIdAsync(Guid id)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null) return null;

            return MapToDto(route);
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var routes = await _routeRepository.GetAllAsync();
            return routes.Select(MapToDto);
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto)
        {
            if (string.IsNullOrWhiteSpace(createRouteDto.Origin))
                throw new ArgumentException("Origin is required.");
            
            if (string.IsNullOrWhiteSpace(createRouteDto.Destination))
                throw new ArgumentException("Destination is required.");

            if (createRouteDto.DistanceKm <= 0)
                throw new ArgumentException("Distance must be greater than 0.");

            var exists = await _routeRepository.ExistsAsync(createRouteDto.Origin, createRouteDto.Destination);
            if (exists)
                throw new Exception($"A route from {createRouteDto.Origin} to {createRouteDto.Destination} already exists.");

            var route = new Route
            {
                Id = Guid.NewGuid(),
                Origin = createRouteDto.Origin,
                Points = createRouteDto.Points,
                Destination = createRouteDto.Destination,
                DistanceKm = createRouteDto.DistanceKm,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _routeRepository.AddAsync(route);
            await _routeRepository.SaveChangesAsync();

            return MapToDto(route);
        }

        public async Task<bool> UpdateRouteAsync(Guid id, UpdateRouteDto updateRouteDto)
        {
            if (string.IsNullOrWhiteSpace(updateRouteDto.Origin))
                throw new ArgumentException("Origin is required.");
            
            if (string.IsNullOrWhiteSpace(updateRouteDto.Destination))
                throw new ArgumentException("Destination is required.");

            if (updateRouteDto.DistanceKm <= 0)
                throw new ArgumentException("Distance must be greater than 0.");

            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null) return false;

            // Check if changing origin/destination leads to a duplicate
            if (route.Origin != updateRouteDto.Origin || route.Destination != updateRouteDto.Destination)
            {
                var exists = await _routeRepository.ExistsAsync(updateRouteDto.Origin, updateRouteDto.Destination);
                if (exists)
                    throw new Exception($"Another route from {updateRouteDto.Origin} to {updateRouteDto.Destination} already exists.");
            }

            route.Origin = updateRouteDto.Origin;
            route.Points = updateRouteDto.Points;
            route.Destination = updateRouteDto.Destination;
            route.DistanceKm = updateRouteDto.DistanceKm;
            route.IsActive = updateRouteDto.IsActive;
            route.UpdatedAt = DateTime.UtcNow;

            _routeRepository.Update(route);
            await _routeRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRouteAsync(Guid id)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null) return false;

            _routeRepository.Delete(route);
            await _routeRepository.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<string> Origins, IEnumerable<string> Destinations)> GetDistinctLocationsAsync()
        {
            return await _routeRepository.GetDistinctLocationsAsync();
        }

        private RouteDto MapToDto(Route route)
        {
            return new RouteDto
            {
                Id = route.Id,
                Origin = route.Origin,
                Points = route.Points,
                Destination = route.Destination,
                DistanceKm = route.DistanceKm,
                IsActive = route.IsActive,
                CreatedAt = route.CreatedAt,
                UpdatedAt = route.UpdatedAt
            };
        }
    }
}
