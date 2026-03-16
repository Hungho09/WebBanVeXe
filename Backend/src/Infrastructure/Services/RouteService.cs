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
            return route != null ? MapToDto(route) : null;
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var routes = await _routeRepository.GetAllAsync();
            return routes.Select(MapToDto);
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto dto)
        {
            var route = new Route
            {
                Id = Guid.NewGuid(),
                Origin = dto.Origin,
                Destination = dto.Destination,
                Points = dto.Points,
                DistanceKm = dto.DistanceKm,
                CreatedAt = DateTime.UtcNow
            };

            await _routeRepository.AddAsync(route);
            await _routeRepository.SaveChangesAsync();

            return MapToDto(route);
        }

        public async Task<bool> UpdateRouteAsync(Guid id, UpdateRouteDto dto)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null) return false;

            route.Origin = dto.Origin;
            route.Destination = dto.Destination;
            route.Points = dto.Points;
            route.DistanceKm = dto.DistanceKm;

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

        private RouteDto MapToDto(Route route)
        {
            return new RouteDto
            {
                Id = route.Id,
                Origin = route.Origin,
                Destination = route.Destination,
                Points = route.Points,
                DistanceKm = route.DistanceKm
            };
        }
    }
}
