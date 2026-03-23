using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface ISeatService
    {
        /// <summary>
        /// Retrieves the seat templates for a specific bus type.
        /// </summary>
        Task<IEnumerable<SeatTemplate>> GetTemplatesByBusTypeAsync(BusType busType);

        /// <summary>
        /// Generates the actual Seat records for a given Trip based on the BusType's layout template.
        /// </summary>
        Task<IEnumerable<Seat>> GenerateSeatsForTripAsync(Guid tripId, BusType busType);
    }
}
