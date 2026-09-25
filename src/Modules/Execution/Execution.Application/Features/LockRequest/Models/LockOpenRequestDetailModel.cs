using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.LockRequest.Models
{
    public sealed record LockOpenRequestDetailModel
    {
        public int ID { get; init; }

        public int LockRequestID { get; init; }

        public DateTime OpenDate { get; init; }

        public string? Reason { get; init; }

        public int Duration { get; init; }

        public int Counts { get; init; }

        public bool IsActive { get; init; }
    }
}
