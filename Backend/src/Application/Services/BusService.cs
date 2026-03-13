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

        public BusService(IBusRepository busRepository)
        {
            _busRepository = busRepository;
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
            var exists = await _busRepository.ExistsByPlateNumberAsync(createBusDto.PlateNumber);
            if (exists)
            {
                throw new Exception("Bus with this plate number already exists.");
            }

            var bus = new Bus
            {
                Id = Guid.NewGuid(),
                PlateNumber = createBusDto.PlateNumber,
                BusType = createBusDto.BusType,
                SeatCapacity = createBusDto.SeatCapacity,
                IsActive = true
            };

            await _busRepository.AddAsync(bus);
            return MapToDto(bus);
        }

        public async Task<bool> UpdateBusAsync(Guid id, UpdateBusDto updateBusDto)
        {
            if (id != updateBusDto.Id) return false;

            var bus = await _busRepository.GetByIdAsync(id);
            if (bus == null) return false;

            // Optional: Check plate number collision if changed
            if (bus.PlateNumber != updateBusDto.PlateNumber)
            {
                var exists = await _busRepository.ExistsByPlateNumberAsync(updateBusDto.PlateNumber);
                if (exists) throw new Exception("Another bus with this plate number already exists.");
            }

            bus.PlateNumber = updateBusDto.PlateNumber;
            bus.BusType = updateBusDto.BusType;
            bus.SeatCapacity = updateBusDto.SeatCapacity;
            bus.IsActive = updateBusDto.IsActive;

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
                PlateNumber = bus.PlateNumber,
                BusType = bus.BusType.ToString(),
                SeatCapacity = bus.SeatCapacity,
                IsActive = bus.IsActive
            };
        }
    }
}
