using Himapp.SharedKernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Himapp.Execution.Domain.Entities
{
    public class LockRequestDetails : BaseEntity
    {
        public int LockRequestID { get; set; }
        public DateTime OpenDate { get; set; }
        public string? Reason { get; set; }
        public int Duration { get; set; }
        public int Counts { get; set; }
        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public virtual LockRequest? LockRequest { get; set; }
    }

}
