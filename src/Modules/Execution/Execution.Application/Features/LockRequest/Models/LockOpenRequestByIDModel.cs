using System;
using System.Collections.Generic;
using System.Text;

namespace Himapp.Execution.Application.Features.LockRequest.Models
{
    public sealed record LockOpenRequestByIDModel
    {
        public int ID { get; init; }

        public Guid UniqueID { get; init; }

        public string? RequestCode { get; init; }

        public DateTime RequestDate { get; init; }

        public string? Remarks { get; init; }

        public int CompanyID { get; init; }

        public int ProjectID { get; init; }

        public int ProgramID { get; init; }

        public short StateID { get; init; }

        public bool IsActive { get; init; }

        public int CreatedBy { get; init; }

        public DateTime CreatedDate { get; init; }

        public int? LastModifiedBy { get; init; }

        public DateTime? LastModifiedDate { get; init; }

        public IReadOnlyCollection<LockOpenRequestDetailModel> Details { get; init; }
            = Array.Empty<LockOpenRequestDetailModel>();
    }
}
