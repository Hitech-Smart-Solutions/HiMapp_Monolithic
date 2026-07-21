using System.Threading.Tasks;

namespace Himapp.SharedKernel.Outbox
{
    public interface IOutboxService
    {
        Task EnqueueAsync(string destination, string payload, string contentType = "application/json");
    }
}
