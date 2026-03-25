using Application.DTOs.Route;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRouteService
    {
        Task<RouteDto?> GetRouteByIdAsync(Guid id);
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
        Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto);
        Task<bool> UpdateRouteAsync(Guid id, UpdateRouteDto updateRouteDto);
        Task<bool> DeleteRouteAsync(Guid id);
        Task<(IEnumerable<string> Origins, IEnumerable<string> Destinations)> GetDistinctLocationsAsync();
    }
}
