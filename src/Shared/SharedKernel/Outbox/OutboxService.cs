using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Himapp.SharedKernel.Outbox
{
    public class OutboxService : IOutboxService
    {
        private readonly OutboxDbContext _db;

        public OutboxService(OutboxDbContext db)
        {
            _db = db;
        }

        public async Task EnqueueAsync(string destination, string payload, string contentType = "application/json")
        {
            var msg = new OutboxMessage
            {
                Destination = destination,
                Payload = payload,
                ContentType = contentType,
                OccurredOnUtc = DateTime.UtcNow
            };

            _db.OutboxMessages.Add(msg);
            await _db.SaveChangesAsync();
        }
    }
}
