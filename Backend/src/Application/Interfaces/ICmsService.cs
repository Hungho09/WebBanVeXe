using System.Threading.Tasks;
using Application.DTOs.Cms;

namespace Application.Interfaces
{
    public interface ICmsService
    {
        Task<CmsConfigDto?> GetConfigAsync(string key);
        Task<bool> UpdateConfigAsync(CmsConfigDto configDto);
    }
}
