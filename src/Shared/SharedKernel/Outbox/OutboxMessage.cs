using System;
using System.ComponentModel.DataAnnotations;

namespace Himapp.SharedKernel.Outbox
{
    public class OutboxMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Destination { get; set; } = string.Empty; // e.g. topic or queue
        public string Payload { get; set; } = string.Empty; // serialized message
        public string ContentType { get; set; } = "application/json";
        public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;
        public bool Dispatched { get; set; } = false;
        public DateTime? DispatchedOnUtc { get; set; }
        public int DispatchAttempts { get; set; } = 0;
    }
}