using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBusTypeService
    {
        Task<IEnumerable<BusTypeDto>> GetAllAsync();
    }
}
