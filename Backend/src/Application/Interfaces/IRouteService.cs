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
    }

    public class RouteDto
    {
        public Guid Id { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Points { get; set; } = string.Empty;
        public int DistanceKm { get; set; }
    }

    public class CreateRouteDto
    {
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Points { get; set; } = string.Empty;
        public int DistanceKm { get; set; }
    }

    public class UpdateRouteDto
    {
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Points { get; set; } = string.Empty;
        public int DistanceKm { get; set; }
    }
}
