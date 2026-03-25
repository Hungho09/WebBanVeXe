using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInvoiceNumberGenerator
    {
        Task<string> GenerateAsync(CancellationToken cancellationToken = default);
    }
}
