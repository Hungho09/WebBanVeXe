using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class BusService : IBusService
    {
        private readonly IBusRepository _busRepository;
        private readonly IApplicationDbContext _context;

        public BusService(IBusRepository busRepository, IApplicationDbContext context)
        {
            _busRepository = busRepository;
            _context = context;
        }

        public async Task<IEnumerable<BusDto>> GetAllBusesAsync()
        {
            var buses = await _busRepository.GetAllAsync();
            return buses.Select(MapToDto);
        }

        public async Task<BusDto?> GetBusByIdAsync(Guid id)
        {
            var bus = await _busRepository.GetByIdAsync(id);
            if (bus == null) return null;
            return MapToDto(bus);
        }

        public async Task<BusDto> CreateBusAsync(CreateBusDto createBusDto)
        {
            var exists = await _busRepository.ExistsByPlateNumberAsync(createBusDto.LicensePlate);
            if (exists)
            {
                throw new Exception("Bus with this plate number already exists.");
            }

            var busType = await _context.BusTypes.FindAsync(createBusDto.BusTypeId);
            if (busType == null)
            {
                throw new Exception("Valid BusType is required.");
            }

            var bus = new Bus
            {
                Id = Guid.NewGuid(),
                PlateNumber = createBusDto.LicensePlate,
                CompanyName = createBusDto.CompanyName,
                ImageUrl = createBusDto.ImageUrl,
                BusTypeId = createBusDto.BusTypeId,
                SeatCount = busType.SeatCount,
                Status = createBusDto.Status
            };

            await _busRepository.AddAsync(bus);
            
            // Reload to get BusType navigation property for MapToDto
            var createdBus = await _busRepository.GetByIdAsync(bus.Id);
            return MapToDto(createdBus!);
        }

        public async Task<bool> UpdateBusAsync(Guid id, UpdateBusDto updateBusDto)
        {
            if (id != updateBusDto.Id) return false;

            var bus = await _busRepository.GetByIdAsync(id);
            if (bus == null) return false;

            if (bus.PlateNumber != updateBusDto.LicensePlate)
            {
                var exists = await _busRepository.ExistsByPlateNumberAsync(updateBusDto.LicensePlate);
                if (exists) throw new Exception("Another bus with this plate number already exists.");
            }

            if (bus.BusTypeId != updateBusDto.BusTypeId)
            {
                var busType = await _context.BusTypes.FindAsync(updateBusDto.BusTypeId);
                if (busType != null)
                {
                    bus.BusTypeId = updateBusDto.BusTypeId;
                    bus.SeatCount = busType.SeatCount;
                }
            }

            bus.PlateNumber = updateBusDto.LicensePlate;
            bus.CompanyName = updateBusDto.CompanyName;
            bus.ImageUrl = updateBusDto.ImageUrl;
            bus.Status = updateBusDto.Status;

            await _busRepository.UpdateAsync(bus);
            return true;
        }

        public async Task<bool> DeleteBusAsync(Guid id)
        {
            var bus = await _busRepository.GetByIdAsync(id);
            if (bus == null) return false;

            await _busRepository.DeleteAsync(bus);
            return true;
        }

        private static BusDto MapToDto(Bus bus)
        {
            return new BusDto
            {
                Id = bus.Id,
                LicensePlate = bus.PlateNumber,
                CompanyName = bus.CompanyName,
                ImageUrl = bus.ImageUrl,
                SeatCount = bus.SeatCount,
                BusType = new BusTypeDto
                {
                    Id = bus.BusType.Id,
                    Name = bus.BusType.Name,
                    SeatCount = bus.BusType.SeatCount
                },
                IsActive = bus.IsActive,
                Status = bus.Status
            };
        }
    }
}
